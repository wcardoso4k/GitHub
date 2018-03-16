"use strict";

// Retorna uma lista de linguagens de programçao
// Retorno da controller: Json formater
function getAllLanguages() {
    $('#listGetAllLanguages').html('<span class="glyphicon-search"> Aguarde...</span>');
    return $.ajax(
        {
            url: "/home/GetAllLanguages",
            type: "GET",
            dataType: "json",
            cache: false,
            success: function (data) {
                var lang = "<ul>";
                $(eval(data)).each(function () {
                    lang += "<li><a href=\"#\">" + this.Name + "</a></li>";
                });
                lang += "</ul>";
                $('#listGetAllLanguages').html(lang);
            },
            error: function (data, textStatus, jqXhr) {
                alert("Erro ao listar Linguagens" + data);
            }
        });
}

$(function () {
    $(".buttonList").click(function () {
        getAllLanguages();
    });
});