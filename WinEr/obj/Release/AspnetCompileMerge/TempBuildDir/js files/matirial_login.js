$(window, document, void 0).ready(function () { $("input").blur(function () { var i = $(this); i.val() ? i.addClass("used") : i.removeClass("used") }); var i = $(".ripples"); i.on("click.Ripples", function (i) { var n = $(this), a = n.parent().offset(), e = n.find(".ripplesCircle"), t = i.pageX - a.left, o = i.pageY - a.top; e.css({ top: o + "px", left: t + "px" }), n.addClass("is-active") }), i.on("animationend webkitAnimationEnd mozAnimationEnd oanimationend MSAnimationEnd", function (i) { $(this).removeClass("is-active") }) });