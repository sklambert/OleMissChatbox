"use strict";


var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let chatClasses = [];

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    getClasses();
}).catch(function (err) {
    return console.error(err.toString());
});

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    var li = document.createElement("li");
    document.getElementById("messagesList").appendChild(li);
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    li.textContent = `${user} says ${message}`;
});

function showCreateClassModal() {
    $('#create-class-modal').modal('show');
    document.getElementById("classNameErrorMessage").hidden = true;
};

function hideCreateClassModal() {
    $('#create-class-modal').modal('hide');
};

function getClasses() {
    console.log("We are in getClasses");
    connection.invoke("GetClasses").then(function (result) {
        document.getElementById("class-list").innerHTML = "";
        for (var i = 0; i < result.length; i++) {
            chatClasses.push(new Class(result[i].className, result[i].createdDate));
            var li = document.createElement("li");
            document.getElementById("class-list").appendChild(li);
            li.textContent = result[i].className;
        }
    });
}

connection.on("addClass", function () {
    getClasses();
});

connection.on("onError", function (message) {
    document.getElementById("error-message").textContent = message;
    $("#errorAlert").removeClass("d-none").show().delay(5000).fadeOut(500);
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});

document.getElementById("create-class").addEventListener("click", function (event) {
    var currentEmail = document.getElementById("email-header").textContent;
    var className = $("#className").val();
    if (className) {
        connection.invoke("CreateClass", currentEmail, className);
        hideCreateClassModal();
        getClasses();
    }
    else {
        document.getElementById("classNameErrorMessage").hidden = false;
    }
    event.preventDefault();
});

function Class(className, createdDate) {
    var self = this;
    self.className = className;
    self.createdDate = createdDate;
}