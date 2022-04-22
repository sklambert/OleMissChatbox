"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
let chatClasses = [];
let chatUsers = [];
let chatMessages = [];
var userType;

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    getClasses();
    var currentEmail = document.getElementById("email-header").textContent;
    connection.invoke("SetUserPermissionLevel", currentEmail);
}).catch(function (err) {
    return console.error(err.toString());
});

connection.on("SetUserPermissionLevel", function (userType) {
    self.userType = userType;
    console.log("connection.on, the usertype is " + self.userType);
    if (userType == 1 || userType == 2) {
        // make addClass button visible
        document.getElementById("add-class").hidden = false;
    }
    else {
        // make it disabled and hidden
        document.getElementById("add-class").disabled = true;
        document.getElementById("add-class").hidden = true;
    }
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
    connection.invoke("GetClasses").then(function (result) {
        document.getElementById("class-list").innerHTML = "";
        for (var i = 0; i < result.length; i++) {
            chatClasses.push(new ChatClass(result[i].className, result[i].createdDate));
            var li = document.createElement("li");
            document.getElementById("class-list").appendChild(li);
            li.textContent = result[i].className;
            li.setAttribute("onclick", "joinClass('" + result[i].className + "')");
        }
    });
}

function joinClass(className) {
    // Invokes the JoinClass method in ChatHub with the class name

    // Then track the name of the joined room (optionally call another JS function to handle this)
    // Get the list of users (do this by calling getUsers() JS function)
    // Get the sorted list of messages (do this by calling getMessages() JS function)


}

function getUsers(className) {
    connection.invoke("GetUsers", className).then(function (result) {
        for (var i = 0; i < result.length; i++) {
            chatUsers.push(new ChatUser(result[i].email, result[i].firstName, result[i].lastName, result[i].currentClass));
        }
    });
}

function getMessages(className) {
    connection.invoke("GetMessages", className).then(function (result) {
        for (var i = 0; i < result.length; i++) {
            chatUsers.push(new ChatMessage(result[i].messageString, result[i].messageSender, result[i].messageDate));
        }
    });
}

connection.on("addUser", function (user) {
    chatUsers.push(user);
});

connection.on("removeUser", function (user) {
    chatUsers.splice(chatUsers.indexOf(user));
});

connection.on("addClass", function () {
    getClasses();
});

connection.on("onError", function (message) {
    $('#error-message-modal').modal('show');
    document.getElementById("user-error-message").textContent = message;
});

function hideErrorMessageModal() {
    $('#error-message-modal').modal('hide');
};

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

function ChatClass(className, createdDate) {
    var self = this;
    self.className = className;
    self.createdDate = createdDate;
}

function ChatUser(email, firstName, lastName, currentClass) {
    var self = this;
    self.email = email;
    self.firstName = firstName;
    self.lastName = lastName;
    self.currentClass = currentClass;
}

function ChatMessage(messageString, messageSender, messageDate) {
    var self = this;
    self.messageString = messageString;
    self.messageSender = messageSender;
    self.messageDate = messageDate;
}