<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ForumControl.ascx.cs" Inherits="TihieZori.Forum.ForumControl" %>
<link href="Forum/forum.css" rel="stylesheet" type="text/css" />
<link href="jqDynaForm/jqDynaForm.css" rel="stylesheet" type="text/css" />
<script src="Forum/forum.js" type="text/javascript" ></script>
<script src="jqDynaForm/jqDynaForm.js" type="text/javascript" ></script>

    <div class="container" id="comment_form">
        <% if (CurUser == null)
            { %>
        <div class="row">
            <label>Имя и Фамилия</label><div class="row"></div><input type="text" name="Author" /></div>
        <div class="row">
            <label>Email-адрес</label><div class="row"></div><input type="text" name="Email" /></div>
        <% } %>
        <div class="row">
            <label>Тема</label><div class="row"></div><input type="text" name="Title"/></div>
        <div class="row">
            <label>Сообщение</label><div class="row"></div><textarea name="message" style="width: 800px;"></textarea></div>
        <div class="row"><input type="button" id="comment_submit" style="width: 240px;margin:20px" value="Добавить сообщение" /></div>
    </div>
