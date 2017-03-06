$(document).ready(function() {
    
    $("#search-button").on("click", search);
    $(document).keypress(function(e) {
        if(e.which === 13) {
            e.preventDefault();
            if ($("#search-box").val() !== "") {
                search();
            }
        }
    });

    $("#results-list").on("click", "li", selectSearchResult);

    $("#person-address").on("click", function(event) {
        hideAllResults();
        $("#search-loading").show();
        var id = $(this).find(".address-id").text();
        showAddress(id);
    });

    $("#address-people-list").on("click", "li", function(event) {
        hideAllResults();
        $("#search-loading").show();
        var id = $(this).find(".person-id").text();
        showPerson(id);
    });

    $("#person-back-to-search, #address-back-to-search").on("click", function() {
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

function search() {
    hideAllResults();
    $("#search-loading").show();

    $.ajax({
        url: "http://localhost:5000/api/search?term=" + $("#search-box").val()
    }).done(function(data) {
        $("#results-list").empty();
        $(data).each(function(index, result) {
            $("#results-list").append("<li>" 
                + result.text 
                + "<span class='search-id'>" + result.id + "</span>"
                + "<span class='search-type'>" + result.type + "</span></li>");
        });

        $("#search-loading").hide();
        $("#results-view").show();
    });
}

function selectSearchResult() {
    hideAllResults();
    $("#search-loading").show();

    var id = $(this).find(".search-id").text();
    var type = $(this).find(".search-type").text();

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
        $("#person-address .address-id").text(data.address.id);
        $("#person-unit-value").text(data.unit);
        $("#person-affiliation-value").text(data.affiliation);
        var date = new Date(data.registrationDate);
        $("#person-registration-value").text(date.toLocaleDateString());

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
        $(data.people).each(function(index, person) {
            $("#address-people-list").append("<li>" 
            + person.fullName
            + (person.unit != "" ? " (" + person.unit + ")" : "")
            + "<span class='person-id'>" + person.id + "</span></li>");
        });

        $("#search-loading").hide();
        $("#address-view").show();
    });
}