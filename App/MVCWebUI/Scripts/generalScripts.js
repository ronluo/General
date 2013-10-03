/////////////////////////////////////////////
// js functions for master page start///////
$(function () {
    // control body size.
    var winWid = $(window).width();
    var winHei = $(window).height();
    var leftwid = $("#bodyleft").width();
    $("#mainbody").height(winHei - 212);
    $("#bodyleft").height(winHei - 212);
    $("#bodyright").height(winHei - 212);
    $("#bodyright").width(winWid - leftwid);
    // control navigation size
    var bodywid = $("#mainbody").width();
    $("#navigationbar").width(bodywid);
    var navmenuwid = $("#navigationmenu").width();
    $("#userprofilepanel").width(bodywid - navmenuwid - 2);
});
// js functions for master page end/////////