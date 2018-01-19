$(document).ready(function () {
    $("#routeViewToggle").click(function () {
        $("#routeViewContainer, #openRouteIcon, #closeRouteIcon").toggle();
    });
    $("#RouteCount").text($("#routeViewContainer li").length);

    $.extend($.expr[":"], {
        "containsIgnoreCase": function (elem, i, match, array) {
            return (elem.textContent || elem.innerText || "").toLowerCase().indexOf((match[3] || "").toLowerCase()) >= 0;
        }
    });

    $("#RouteViewFilter").keyup(function () {
        $("#routeViewContainer").find("li:not(:containsIgnoreCase('" + $(this).val() + "'))").hide();
        $("#routeViewContainer").find("li:containsIgnoreCase('" + $(this).val() + "')").show();
    });
});