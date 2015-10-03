function messageViewModel(username, text) {
    var self = this;

    self.username = ko.observable(username);
    self.text = ko.observable(text);
}

function userViewModel(username) {
    var self = this;

    self.username = ko.observable(username);
}

function chatViewModel() {
    var self = this;

    self.hub = $.connection.chatHub;
    self.error = ko.observable('');
    self.username = ko.observable().extend({ required: true, minLength: 5 });
    self.joined = ko.observable(false);
    self.newMessageText = ko.observable().extend({ required: true });
    self.users = ko.observableArray();
    self.messages = ko.observableArray();

    self.userScrolled = false;
    self.autoScrolling = false;

    self.hub.client.invalidOperation = function (errorMessage) {
        self.error(errorMessage);
    };

    self.hub.client.joinedSuccessfully = function (userList, recentMessages) {
        var mappedUsers = $.map(userList, function (usr) {
            return new userViewModel(usr);
        });
        self.users(mappedUsers);

        var mappedMessages = $.map(recentMessages, function (msg) {
            return new messageViewModel(msg.UserName, msg.Text);
        });
        self.messages(mappedMessages);
        self.joined(true);
        $("#msg").focus();
    }

    self.hub.client.leftSuccessfully = function () {
        self.joined(false);
    }

    self.hub.client.newUserJoined = function (username) {
        self.users.push(new userViewModel(username));
    };

    self.hub.client.userLeft = function (username) {
        self.users.remove(function (item) {
            return item.username() === username;
        });
    };

    self.hub.client.newMessage = function (username, message) {
        self.messages.push(new messageViewModel(username, message));
        self.adjustScroll();
    };


    self.sendMessage = function () {
        if (!self.newMessageText.isValid()) {
            return;
        }

        self.hub.server.sendMessage(this.newMessageText());
        self.newMessageText("");
    };

    self.join = function () {
        if (!self.username.isValid()) {
            return;
        }
        self.messages.removeAll();
        self.users.removeAll();
        self.hub.server.join(self.username());
    };

    self.leave = function () {
        self.hub.server.leave();
    };

    self.adjustScroll = function () {
        if (!self.userScrolled) {
            self.autoScrolling = true;
            var container = $("#messagesContainer");
            container.animate({ scrollTop: container.prop("scrollHeight") }, 500);
            self.autoScrolling = false;
        }
    };

    self.handleUserScroll = function () {
        if (self.autoScrolling) {
            return;
        }

        var container = $("#messagesContainer");
        if (container.scrollTop() + container.innerHeight() >= container.prop("scrollHeight")) {
            self.userScrolled = false;
        } else {
            self.userScrolled = true;
        }
    };
};