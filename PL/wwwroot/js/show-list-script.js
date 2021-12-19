$(document).ready(function () {
    $('#list > li').click(function (event) {
        $(this).children("ul").slideToggle();
        event.stopPropagation();
    });
});

$(document).ready(function () {
    $('#list > li > ul > li').click(function (event) {
        $(this).children("ul").slideToggle();
        event.stopPropagation();
    });
});

$(document).ready(function () {
    $('#list > li > ul > li > ul > li').click(function (event) {
        $(this).children("ul").slideToggle();
        event.stopPropagation();
    });
});

$(document).ready(function () {
    $('#list > li > ul > li > ul > li > ul > li').click(function (event) {
        $(this).children("ul").slideToggle();
        event.stopPropagation();
    });
});
