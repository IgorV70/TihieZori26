using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Text;
using DbCommon;
using TihieZoriDb;
using TihieZori.Code;
using Newtonsoft.Json;
using DbCommon.Attributes;
using System.Web.UI;

namespace TihieZori
{
    [ValidationProperty("Comment")]
    public class JsonGateway : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get { return true; }
        }

        CDatabaseTihieZori _db;
        iTable _table;
        HttpRequest _request;
        HttpResponse _response;

        public void ProcessRequest(HttpContext context)
        {
            _request = context.Request;
            _response = context.Response;
            _response.ContentType = "text/json";

            _db = (CDatabaseTihieZori)context.Session["database"];
            string[] pathElements =
                _request.Path.Split(new char[] { '/', '.' }, StringSplitOptions.RemoveEmptyEntries);
            if (pathElements.Length != 3 && pathElements.Length != 5)
            {
                _response.Write("{\"error\":\"некорректный url\"}");
                return;
            }
            if (pathElements[pathElements.Length - 1] != "dbo")
            {
                _response.Write("{\"error\":\"некорректный url\"}");
                return;
            }
            // head/TableName.dbo
            // data/TableName.dbo
            // add/TableName.dbo
            // data/TableName.Id.dbo
            // data/TableName.Id.PropertyListName.dbo
            // head/TableName.Id.PropertyListName.dbo

            string requestType = pathElements[0];
            if (requestType == "cust")
            {
            }

            string tableName = pathElements[1];
            if (_db.Tables.TryGetValue(tableName, out _table))
            {
                switch (requestType)
                {
                    case "dataf":
                        PrepareDataf();
                        return;
                    case "head":
                        PrepareHeader();
                        return;
                    case "data":
                        if (pathElements.Length == 3)
                        {
                            PrepareData();
                            return;
                        };
                        {
                            int id = 0;
                            if (int.TryParse(pathElements[2], out id))
                            {
                                PrepareChildData(id, pathElements[3]);
                                return;
                            }
                        }
                        break;
                    case "edit":
                        {
                                string parId = _request.Params["Id"];
                                int id = 0;
                                if (!int.TryParse(parId, out id)) return;
                                BObject obj = _table.GetById(id) as BObject;
                                if (obj == null) return;
                                obj.UpdateFrom(_request.Params.AllKeys.ToDictionary(k => k, k => _request.Params[k]));
                                obj.Save2();
                                _response.Write(obj.ToJsonArray(null));
                                return;
                        }
                    case "del":
                        {
                            string parId = _request.Params["Id"];
                            int id = 0;
                            if (!int.TryParse(parId, out id)) return;
                            BObject obj = _table.GetById(id) as BObject;
                            if (obj == null) return;
                            obj.MarkForDeletion();
                            obj.Save2();
                            _response.Write("\"" + id + "\"");
                            return;
                        }
                    case "add":
                        {
                            BObject obj = _table.CreateInstance();
                            obj.UpdateFrom(_request.Params.AllKeys.ToDictionary(k => k, k => _request.Params[k]));
                            obj.Save2();
                            var sb = obj.ToJsonArray();
                            _response.Write(sb);
                            return;
                        }
                    default:
                        break;
                }
            }
            _response.Write("{\"error\":\"некорректный url\"}");
        }

        // возвращает строки в виде массивов, первой строкой - наименования столбцов
        private void PrepareDataf()
        {
            string parFields = _request["fields"]; // { fields: "Id,Name" }
            string parWhere = _request["where"];
            string parOrder = _request["order"];
            string parValues = _request["values[]"];

            iSortList dataList;
            if (parWhere != null)
            {
                var jsonCusrtomFiltr = JsonConvert.DeserializeObject<JsonCusrtomFiltr>(parWhere);
                ICustomFiltr customFiltr = _table.GetCustomFiltr(jsonCusrtomFiltr.Name, jsonCusrtomFiltr.Params);
                dataList = _table.GetObjectListByCustom(customFiltr);
            }
            else
            {
                dataList = _table.GetObjectList(new CQueryDesc(_table));
            }
            // заголовок
            StringBuilder sb = new StringBuilder();
            BuildNamesRow(sb, parFields);

            foreach (BObject obj in dataList)
            {
                obj.ToJsonArray(sb, parFields).Append(",");
            }
            sb.Length--;
            sb.Append("]");
            _response.Write(sb);
        }

        /// <summary>
        /// вернет массив с наименованиями столбцов
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="customFields">
        /// строка с наименованиями столбцов через запятую: "Id,Name"
        /// </param>
        private void BuildNamesRow(StringBuilder sb, string customFields)
        {
            sb.Append("[[");
            if (!string.IsNullOrEmpty(customFields))
            {
                // запаковываем в кавычки и все  "Id,Name" => "[["Id","Name"],"
                customFields = customFields.Replace(",", "\",\"");
                sb.Append("\"").Append(customFields).Append("\"");
                sb.Append("],");
                return;
            }
            var fields = _table.Columns.Values;
            foreach (var field in fields)
                sb.Append("\"").Append(field.Name).Append("\"").Append(",");
            foreach (var attr in _table.GetRowType().GetCustomAttributes(true))
            {
                var dopField = attr as AdditionalJsonPropertyAttribute;
                if (dopField != null)
                {
                    sb.Append("\"").Append(dopField.PropertyName).Append("\"").Append(",");
                }
            }
            sb.Length--;
            sb.Append("],");
        }

        private void PrepareData()
        {
            string parFields = _request["fields"];
            string parWhere = _request["where"];
            string parOrder = _request["order"];
            string parValues = _request["values[]"];

            iSortList dataList;
            if (parWhere != null)
            {
                var jsonCusrtomFiltr = JsonConvert.DeserializeObject<JsonCusrtomFiltr>(parWhere);
                ICustomFiltr customFiltr = _table.GetCustomFiltr(jsonCusrtomFiltr.Name, jsonCusrtomFiltr.Params);
                dataList = _table.GetObjectListByCustom(customFiltr);
            }
            else
            {
                dataList = _table.GetObjectList(new CQueryDesc(_table));
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            foreach (BObject obj in dataList)
            {
                obj.ToJsonArray(sb, parFields);
                sb.Append(",");
            }

            if (dataList.GetCount() > 0)
                sb.Length--;
            sb.Append("]");
            _response.Write(sb);
        }

        private string GetFilter()
        {
            string filter = _request["filter[oper]"];
            if (string.IsNullOrEmpty(filter))
                return string.Empty;
            string ret = BuildFilter("filter");
            return ret;
        }

        private string BuildFilter(string path)
        {
            string pathOper = path + "[oper]";
            string filter = _request[pathOper];
            if (string.IsNullOrEmpty(filter))
                return string.Empty;
            string ret = string.Empty;
            switch (filter)
            {
                case "cn":
                    {
                        string name = _request[path + "[field]"];
                        string val = _request[path + "[value]"];
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(val))
                        {
                            name = QuoteName(name);
                            val = Quote(val);
                            ret += name + " like " + "'%" + val + "%'";
                        }
                    }
                    break;
                case "eq":
                    //ret += _request["filter[field]"] + "=" + "'" + _request["filter[value]"] + "'";
                    {
                        string name = _request[path + "[field]"];
                        string val = _request[path + "[value]"];
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(val))
                        {
                            name = QuoteName(name);
                            val = Quote(val);
                            ret += name + "=" + "'" + val + "'";
                        }
                    }
                    break;
            }
            return ret;
        }

        private string Quote(string name)
        {
            return name.Replace("'", "''");
        }

        private string QuoteName(string name)
        {
            return name.Replace("'", "");
        }

        private void PrepareHeader()
        {
            var fields = _table.Columns.Values;
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            foreach (var field in fields)
            {
                sb.Append("\"").Append(field.Name).Append("\"");
                //field.ToJson(sb);
                sb.Append(",");
            }
            sb.Length--;
            sb.Append("]");
            _response.Write(sb);
        }


        private void PrepareChildData(int id, string propertyListName)
        {
            string parSearch = _request["_search"];
            string parNd = _request["nd"];
            string parRows = _request["rows"];
            string parPage = _request["page"];
            string parSidx = _request["sidx"];
            string parSord = _request["sord"];

            //string sortOrder = string.IsNullOrEmpty(par_sidx) ? "Id" : (par_sidx + (par_sord == "desc" ? " desc" : ""));
            //CQueryDesc qd = new CQueryDesc(_table, sortOrder);


            var parentObj = _table.GetById(id);

            IISortList<BObject> dataList = (parentObj as BObject).GetListProperty(propertyListName);

            string parOper = _request["oper"];
            if (!string.IsNullOrEmpty(parOper))
            {
                ProcessOperation(parOper, dataList);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");

            //var fields = _table.GetFieldsDescription();

            foreach (BObject obj in dataList)
            {
                obj.ToJsonArray(sb);
                sb.Append(',');
            }

            if (dataList.GetCount() > 0)
                sb.Length--;
            sb.Append("]");
            _response.Write(sb);
        }


        private void ProcessOperation(string parOper, IISortList<BObject> dataList)
        {
            if (parOper == "edit")
            {
                string parId = _request.Params["Id"];
                int id = 0;
                int.TryParse(parId, out id);
                //var oo = dataList.F
                BObject obj = dataList.FirstOrDefault(o => ((IPrimaryKey<int>)o).Id == id);
                if (obj != null)
                {
                    obj.UpdateFrom(_request.Params.AllKeys.ToDictionary(k => k, k => _request.Params[k]));
                    obj.Save2();
                    var sb = obj.ToJsonArray();
                    _response.Write(sb);
                }
                return;
            }
            if (parOper == "del")
            {
                string parId = _request.Params["id"];
                int id = 0;
                int.TryParse(parId, out id);
                BObject obj = dataList.FirstOrDefault(o => ((IPrimaryKey<int>)o).Id == id);
                dataList.Remove(obj);
                dataList.Save();
                return;
            }
            if (parOper == "add")
            {
                BObject obj = dataList.Table.CreateInstance();
                obj.UpdateFrom(_request.Params.AllKeys.ToDictionary(k => k, k => _request.Params[k]));
                obj.Save2();
                dataList.Add(obj);

                var sb = obj.ToJsonArray();
                _response.Write(sb);

            }
        }



    }
}
