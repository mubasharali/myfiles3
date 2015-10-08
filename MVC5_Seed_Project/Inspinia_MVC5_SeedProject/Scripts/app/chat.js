function OnlineUsers(data) {
    var self = this;
    data = data || {};
    self.id = data.id;
    self.name = data.name;
    self.dpExtension = data.dpExtension;
    self.sendTo = function () {
        alert("done");
    }
}
function Message(data) {
    var self = this;
    data = data || {};
    self.id = data.Id;
    self.name = data.name;
    self.sentFrom = data.from;
    self.sentTo = data.to;
    self.sentFromName = data.sentFromName;
    self.sentToName = data.sentToName;
    self.message = data.message;
    self.time = (data.time);
    self.timeAgo = getTimeAgo(data.time);
}
function sendMessageTo(id) {
    $.ajax({
        url: '/api/Chat/SendMessageTo?id=' + id,
        dataType: "json",
        contentType: "application/json",
        cache: false,
        type: 'POST',
        success: function (data) {
            alert("ok");
        },
        error: function () {
            toastr.error("failed to accept message sending to this user", "Error!");
        }
    });
}

function ChatViewModel() {
    var self = this;
    self.hub = $.connection.chatHub;
    self.onlineUsersHub = $.connection.onlineUsers;
    self.showChat = ko.observableArray();
    self.newMessage = ko.observable();
    self.loginUserId = true;
    self.onlineUsers = ko.observableArray();
    self.sendMessageTo = ko.observable();
    self.sendTo = function (data) {
        self.sendMessageTo(data.id);
        console.log("Message will be sent to: " + data.name);
    }
    self.checkMsgEnter = function (d, e) {
        if (self.loginUserId) {
            if (self.newMessage().length < 1000) {
                if (e.keyCode == 13) {
                    self.sendMessage();
                }
            } else {
                self.newMessage(self.newMessage().slice(0, -1));
                toastr.info("You reached the limit", "Message too long!");
            }
        } else {
            $("#inputEmail").modal('show');
        }
    }
    self.hub.client.loadNewMessage = function (data) {
        self.newMessage('');
        if (data != null) {
            self.showChat.push(new Message(data));
        }
    }
    self.onlineUsersHub.client.showConnected = function (connectionId) {
        var mape = $.map(connectionId, function (item) { return new OnlineUsers(item) });
        self.onlineUsers(mape);
    }
    self.getReceiverId = function (email) {
        $.ajax({
            url: '/api/Chat/GetIdByEmail?email=' + email,
            dataType: "json",
            contentType: "application/json",
            cache: false,
            type: 'POST',
            success: function (data) {
                return data;
            },
            error: function () {
                toastr.error("failed to send message", "Error!");
                return null;
            }
        });
    }
    self.sendMessage = function () {
        var msg = new Message();
        // msg.sentTo = "287c5c50-c632-41f7-811c-6939ff23f331";
        msg.sentTo = self.sendMessageTo();
        msg.message = self.newMessage();
        self.hub.server.addMessage(msg).fail(function (err) { toastr.error("failed to send message", "Error!"); });
    }
}