﻿!function (n) { n.movingBoxes = function (e, i) { var t = this; t.$el = n(e), t.el = e, t.$el.data("movingBoxes", t), t.init = function () { t.options = n.extend({}, n.movingBoxes.defaultOptions, i); var e = t.$el.find(".panel"); t.options.totalPanels = e.length, t.options.regWidth = e.css("width"), t.options.regImgWidth = e.find("img").css("width"), t.options.regTitleSize = e.find("h2").css("font-size"), t.options.regParSize = e.find("p").css("font-size"), e.css({ "float": "left", position: "relative" }), t.$el.data("currentlyMoving", !1), t.$el.find(".scrollContainer").css("width", t.$el.find(".panel")[0].offsetWidth * t.$el.find(".panel").length + 150), t.$el.find(".scroll").css("overflow", "hidden"), t.$el.data("currentPanel", -1), t.change(t.options.startPanel), curPanel = t.options.startPanel, t.$el.find(".right").click(function () { t.change(t.$el.data("currentPanel") + 1) }).end().find(".left").click(function () { t.change(t.$el.data("currentPanel") - 1) }), e.click(function () { t.change(e.index(n(this)) + 1) }) }, t.returnToNormal = function (n) { t.$el.find(".panel").not(":eq(" + (n - 1) + ")").animate({ width: t.options.regWidth }, t.options.speed).find("img").animate({ width: t.options.regImgWidth }, t.options.speed).end().find("h2").animate({ fontSize: t.options.regTitleSize }, t.options.speed).end().find("p").animate({ fontSize: t.options.regParSize }, t.options.speed) }, t.growBigger = function (n) { t.$el.find(".panel").eq(n - 1).animate({ width: t.options.curWidth }, t.options.speed).find("img").animate({ width: t.options.curImgWidth }, t.options.speed).end().find("h2").animate({ fontSize: t.options.curTitleSize }, t.options.speed).end().find("p").animate({ fontSize: t.options.curParSize }, t.options.speed) }, t.change = function (n) { var e = t.$el.find(".panel"); if (1 > n || t.$el.data("currentPanel") == n || n > e.length) return !1; if (!t.$el.data("currentlyMoving")) { t.$el.data("currentPanel", n), t.$el.data("currentlyMoving", !0); var i = t.$el.find(".scroll").innerWidth() / 2 - e.eq(n - 1).outerWidth() / 2, o = i - t.options.movingDistance * (n - 1); t.$el.find(".scrollContainer").stop().animate({ left: o }, t.options.speed, function () { t.$el.data("currentlyMoving", !1) }), t.returnToNormal(n), t.growBigger(n) } }, t.currentPanel = function (n) { return "undefined" != typeof n && t.change(parseInt(n, 10)), t.$el.data("currentPanel") }, t.init() }, n.movingBoxes.defaultOptions = { startPanel: 1, movingDistance: 294, curWidth: 300, curImgWidth: 275, curTitleSize: "20px", curParSize: "15px", speed: 500 }, n.fn.movingBoxes = function (e) { return this.each(function () { new n.movingBoxes(this, e) }) }, n.fn.getmovingBoxes = function () { this.data("movingBoxes") } }(jQuery);