var mapSourceBase = "https://www.google.com/maps/embed/v1/place?key=AIzaSyB3O9zJ3W69fqz-N4cTMvfb785HlxqIuoU&q=";

$(document).ready(function() {
    
    $("#search-button").on("click", search);

    $(document).keypress(function(event) {
        if(event.which === 13) {
            search(event);
        }
    });

    $("#results-list").on("click", "li", selectSearchResult);

    $("#person-address").on("click", function(event) {
        event.preventDefault();
        hideAllResults();
        $("#search-loading").show();
        var id = $(this).find(".address-id").val();
        showAddress(id);
    });

    $("#address-people-list").on("click", "li", function(event) {
        event.preventDefault();
        hideAllResults();
        $("#search-loading").show();
        var id = $(this).find(".person-id").val();
        showPerson(id);
    });

    $(".back-to-search").on("click", function() {
        event.preventDefault();
        hideAllResults();
        $("#results-view").show();
    })
});

function hideAllResults() {
        $("#results-view").hide();
        $("#person-view").hide();
        $("#address-view").hide();
        $("#search-loading").hide();
}

function search(event) {
    event.preventDefault();
    if ($("#search-box").val() != "") {
        hideAllResults();
        $("#search-loading").show();

        $.ajax({
            url: "http://localhost:5000/api/search?term=" + $("#search-box").val()
        }).done(function(data) {
            $("#results-list").empty();
            $(data).each(function(index, result) {
                $("#results-list").append("<li><a href='#'>" 
                    + result.text 
                    + "<input type='hidden' class='search-id' value='" + result.id + "'>"
                    + "<input type='hidden' class='search-type' value='" + result.type + "'></a></li>");
            });

            $("#search-loading").hide();
            $("#results-view").show();
        });
    }
}

function selectSearchResult(event) {
    event.preventDefault();
    hideAllResults();
    $("#search-loading").show();

    var id = $(this).find(".search-id").val();
    var type = $(this).find(".search-type").val();

    if (type === "PERSON") {
        showPerson(id);
    } else if (type === "ADDRESS") {
        showAddress(id);
    }
}

function showPerson(id) {
    $.ajax({
        url: "http://localhost:5000/api/person/" + id
    }).done(function(data) { 
        $("#person-name-value").text(data.fullName);
        $("#person-address-value").text(data.address.fullAddress);
        $("#person-address .address-id").val(data.address.id);
        if (data.unit != "") {
            $("#person-unit").show();
            $("#person-unit-value").text(data.unit);
        } else {
            $("#person-unit").hide();
        }
        $("#person-affiliation-value").text(data.affiliation);
        var date = new Date(data.registrationDate);
        $("#person-registration-value").text(date.toLocaleDateString());
        var mapQuery = data.address.fullAddress.replace(/\s/g, "+") + "+Washington+DC";
        $("#person-map iframe").attr("src", mapSourceBase + mapQuery);

        $("#search-loading").hide();
        $("#person-view").show();
    });
}

function showAddress(id) {
    $.ajax({
        url: "http://localhost:5000/api/address/" + id
    }).done(function(data) { 
        $("#address-street-value").text(data.fullAddress);
        $("#address-precinct-value").text(data.precinct);
        $("#address-ward-value").text(data.ward);
        $("#address-people-list").empty();
        var mapQuery = data.fullAddress.replace(/\s/g, "+") + "+Washington+DC";
        $("#address-map iframe").attr("src", mapSourceBase + mapQuery);
        $(data.people).each(function(index, person) {
            $("#address-people-list").append("<li><a href='#'>" 
            + person.fullName
            + "</a>"
            + (person.unit != "" ? " (" + person.unit + ")" : "")
            + "<input type='hidden' class='person-id' value='" + person.id + "'></li>");
        });

        $("#search-loading").hide();
        $("#address-view").show();
    });
}