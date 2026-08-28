<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Editor.aspx.cs" Inherits="TihieZori.Editor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Autosize Demo - CLEditor WYSIWYG HTML Editor</title>
    <link rel="stylesheet" type="text/css" href="cleditor/jquery.cleditor.css" />
      <link href="jqDynaForm/jqDynaForm.css" rel="stylesheet" type="text/css" />    
    <style type="text/css">
      html, body {margin:0; padding:0; overflow:hidden}
      #container {position:absolute}
    </style>
    <script type="text/javascript" src="Scripts/jquery-1.10.2.js"></script>
    <script type="text/javascript" src="cleditor/jquery.cleditor.min.js"></script>
    <script type="text/javascript" src="cleditor/jquery.cleditor.advancedtable.min.js"></script>
    <script src="jqDynaForm/jqDynaForm.js" type="text/javascript"></script>   
    <script type="text/javascript">

        function SaveForm() {
            var data = $('#spform').jqDynaForm('get');
            data = $.extend(data, { "source": "SitePage", "id": current, "action": "save", "PageText": editor.doc.body.innerHTML });
            data.Flags = data.Forum;
            data.Active |= data.Publish << 1;
            $.post('datapath'
                , data
                , function (d) {
                    d = $.parseJSON(d);
                    LoadForm(d.Id);
                    alert("Успешно сохранено !");
                });
        };

        function LoadForm(formId) {
            var data_complete = function (data) {
                if (data) {
                    data.Forum = data.Flags & 1;
                    data.Publish = data.Active & 2;
                    current = formId;
                    $('#spform').jqDynaForm('set', data);
                    $('#formslist').hide();
                    $('#formeditor').show();
                    //editor.doc.body.innerHTML = data.PageText;
                    editor.$area.val(data.PageText);
                    $(window).resize();
                }
            };
            if (formId == -1) {
                data_complete({ "id": -1, "Name": "NewForm.aspx", "Title": "Новая форма", "MasterPage": "SiteGpsrf", "Flags": "1", "Comment": "Новая форма", "Keywords": "", "PageText": "" });
                return;
            }
            $.getJSON('datapath', { "source": "SitePage", "id": formId, "fields": 'id,Name,Title,MasterPage,Flags,Comment,PageText,Keywords,Vers,DatM,Active' }, data_complete);

        }

        $(document).ready(function () {

            $.getJSON('datapath', { "list": "SitePage" }, function (list) {
                var row = $('#pages>tbody>tr:last');
                //alert(table.length);
                for (var i = 0; i < list.length; i++) {
                    row.before('<tr id="row"><td id="cell_name" title="Название">' + list[i].Name
                    + '</td><td id="cell_descr" title="Описание">' + list[i].Comment
                    + '</td><td><img class="formedit" id="formedit' + list[i].Id + '" src="img/car_edit.png" alt="Редактировать" title="Редактировать" border="0"/></td><td><a  id="cell_delet' + list[i].Id + '" class="cell_delete"><img src="img/car_delete.png" alt="Удалить" title="Удалить" border="0" /></a></td></tr>');
                }
                $(".formedit").on('click', function () {
                    var formId = parseInt(this.Id.substr("formedit".length));
                    LoadForm(formId);
                });

            });

            // Define the table button
            $.cleditor.buttons.save = {
                name: "save",
                image: "save.png",
                title: "Save Document",
                command: "inserthtml",
                popupName: "save",
                popupClass: "cleditorPrompt",
                popupContent: "",
                buttonClick: function (e) {
                    window.onbeforeunload = null;
                    SaveForm();
                }
            };

            // Add the button to the default controls
            $.cleditor.defaultOptions.controls = $.cleditor.defaultOptions.controls.replace("bold", "save bold");

            editor = $("#input").cleditor({ width: "100%", height: "100%" })[0].focus();
            //$(window).resize();
            $("#spform").jqDynaForm();

            // Table button click event handler

            //window.onbeforeunload = SaveForm;


            $("#formadd").click(function () {
                LoadForm(-1);
            });

        });

        $(window).resize(function () {
            var $win = $(window);
            $("#container").width($win.width() - 32).height($win.height() - 133).offset({ left: 15, top: 100 });
            editor.refresh();
        });
    </script>
  </head>
  <body  >
  <center id="formslist">
  <table id="pages" border="1" style="margin-left:3px">
<tr><th>&nbsp;&nbsp;&nbsp;Название&nbsp;&nbsp;&nbsp;</th>
    <th>&nbsp;&nbsp;&nbsp;Описание&nbsp;&nbsp;&nbsp;</th>
    <th>&nbsp;Изменить&nbsp;</th>
    <th>&nbsp;Удалить&nbsp;</th>
</tr>

    <tr >
        <td>&nbsp;</td>
        <td >&nbsp;</td>
        <td>
              <img id="formadd" src="img/edit.svg" alt="Добавить" title="Добавить" border="0"/>
       </td>
       <td >&nbsp;</td>
     </tr>
</table>
</center>
<div id="formeditor" style="display:none">
<div id="spform">
<table border="0" cellpadding="2" cellspacing="2">
 <tbody>
  <tr><td>Название формы :</td><td><input name="Name" type=text /></td><td>Заголовок :</td><td><input name="Title" style="width:400px" type=text /></td></tr>
  <tr><td>Шаблон страницы :</td><td><input name="MasterPage" type=text /></td><td>Ключевые слова :</td><td><input name="Keywords" style="width:400px" type=text /></td></tr>
  <tr><td>Комментарий :</td><td><input name="Comment" type=text /></td><td>Флаги :</td><td><input name="Forum" type=checkbox /><input name="Publish" type=checkbox /></td></tr>
  </tbody>
</table><br/>
</div>
    <div id=container>       
      <textarea id="input" name="input">%text%</textarea>
    </div>
</div>
  </body>
</html> 
