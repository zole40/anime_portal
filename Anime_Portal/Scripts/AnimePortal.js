"use strict";
var AnimePortal = window.AnimePortal || {};

AnimePortal.GetComments = function (url, CommentId, type) {
    /// <summary>
    /// Betölti az adott könyvhöz tartozó kommenteket.
    /// </summary>
    $.ajax({
        url: url,
        data: { CommentId: CommentId , type : type },
        dataType: "html",
        type: "GET",
        cache: false,
    }).done(function (data, textStatus, jqXHR) {
        $("#comments").html(data);
    });
};