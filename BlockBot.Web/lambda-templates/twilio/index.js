class Calendar {
    constructor(name, id) {
        this.name = name;
        this.id = id;
    }
}

const request = require('request');

var AWS = require("aws-sdk");

var deasync = require('deasync');

module.exports.handler = (event, context, callback) => {
    const twilio_request = {};
    //twilio_request.Body = "test";
    //twilio_request.From = "Harley";
    const body_array = event.body.split('&');
    for (var i = 0; i < body_array.length; i++){
        var x = body_array[i].split('=');
        twilio_request[x[0]] = x.length > 1 ? decodeURIComponent(x[1]) : null
    }
    console.log(twilio_request);

    // Constants
    const BLOCKBOT_NORMALIZED_USERNAME = "##NORMALIZED_USERNAME##"; // "HARLEY@WALDSTEIN.IO";
    const BLOCKBOT_PROJECT_ID = "##PROJECT_ID##"; //"93800126-dd9b-407f-9d17-852144ba542c";
    const AWS_ACCESS_KEY = "AKIAIXKEQYRQWLKZSQDA";
    const AWS_ACCESS_SECRET = "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";

    // configure DynamoDB client
    AWS.config.update({
        accessKey: AWS_ACCESS_KEY,
        secretAccessKey: AWS_ACCESS_SECRET,
        region: "us-east-1" });

    var ddb = new AWS.DynamoDB({apiVersion: '2012-10-08'});

    var optionPrompt = "";
    var response_message = `<?xml version="1.0" encoding="UTF-8" ?><Response>`


    //
    // Message functions
    //
    function addMessage(messageText) {
        response_message += "<Message>" + messageText + "</Message>";
    }

    function getMessageBody() {
        return twilio_request.Body;
    }

    function getMessageSender() {
        return twilio_request.From;
    }

    //
    // Control flow helper functions
    //
    function startConversation() {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Item: {
                'sender' : {S: getMessageSender()},
            },
            ConditionExpression: 'attribute_not_exists(sender)',
            // ExpressionAttributeNames: {'#i' : 'sender'},
            // ExpressionAttributeValues: {':val' : getMessageSender()}
        };
        ddb.putItem(params, function(err, result) {
            if (err) {
                // item exists
            } else {
                //console.log("success");
            }
            data = result;
            sync = false;
        });
        while(sync) {deasync.sleep(100);}
    }

    function sendMessage() {
        response_message += '</Response>';

        callback(null, {
            statusCode: 200,
            headers: {"content-type": "application/xml"},
            body: response_message
        });
        // TODO see if we can just call callback process.exit();
    }

    function endConversation() {
        var sync = true;
        var data;
        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                'sender' : {S: getMessageSender()},
            }
        };

        ddb.deleteItem(params, function(err, result){
            data = result;
            sync = false;
        });
        while(sync) {deasync.sleep(100);}

        sendMessage();
    }


    //
    // Option helper functions: Unseen -> Pending -> Selected
    //
    function isOptionSelectionUnseen(optionPrompt) {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                'sender' : {S: getMessageSender()},
            }
        };

        ddb.getItem(params, function(err, result) {
            if (err) {
                // TODO handle this?
            } else {
                data = result;
                sync = false;
            }
        });
        while(sync) {deasync.sleep(100);}

        if (data.Item.hasOwnProperty(optionPrompt)){
            return false;
        }

        return true;
    }


    function isOptionSelectionPending(optionPrompt) {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                'sender' : {S: getMessageSender()},
            }
        };

        ddb.getItem(params, function(err, result) {
            if (err) {
                // TODO handle this?
            } else {
                data = result;
                sync = false;
            }
        });
        while(sync) {deasync.sleep(100);}

        if (data.Item[optionPrompt]["S"] === 'pending'){
            return true;
        }

        return false;
    }

    function setOptionSelectionPending(optionPrompt) {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                sender: {S: getMessageSender()}
            },
            UpdateExpression: "set #o = :status",
            ExpressionAttributeNames: {
                "#o": optionPrompt,
            },
            ExpressionAttributeValues: {
                ":status": {
                    "S": "pending"
                }
            }

        };

        ddb.updateItem(params, function(err, result) {
            if (err) {
                console.log(err);
                sync = false;
            } else {
                data = result;
                sync = false;
            }
        });
        while(sync) {deasync.sleep(100);}
    }

    function isOptionSelectionSelected(optionPrompt) {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                'sender' : {S: getMessageSender()},
            }
        };

        ddb.getItem(params, function(err, result) {
            if (err) {
                // TODO handle this?
            } else {
                data = result;
                sync = false;
            }
        });
        while(sync) {deasync.sleep(100);}

        if (data.Item[optionPrompt]["S"] === 'selected'){
            return true;
        }

        return false;
    }

    function setOptionSelectionSelected(optionPrompt, optionValue) {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                sender: {S: getMessageSender()}
            },
            UpdateExpression: "set #o = :status",
            ExpressionAttributeNames: {
                "#o": optionPrompt,
            },
            ExpressionAttributeValues: {
                ":status": {
                    "S": "selected"
                }
            }

        };

        ddb.updateItem(params, function(err, result) {
            if (err) {
                console.log(err);
                sync = false;
            } else {
                data = result;
                sync = false;
            }
        });
        while(sync) {deasync.sleep(100);}
    }

    function getOptionSelected(optionPrompt) {
        // TODO return value set for optionPrompt
    }

    //
    // Variable helper functions
    //
    function setVariable(variableName, variableValue) {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                sender: {S: getMessageSender()}
            },
            UpdateExpression: "set #o = :status",
            ExpressionAttributeNames: {
                "#o": variableName,
            },
            ExpressionAttributeValues: {
                ":status": {
                    "S": variableValue
                }
            }

        };

        ddb.updateItem(params, function(err, result) {
            if (err) {
                console.log(err);
                sync = false;
            } else {
                data = result;
                sync = false;
            }
        });
        while(sync) {deasync.sleep(100);}
    }

    function getVariable(variableName) {
        var sync = true;
        var data = null;

        var params = {
            TableName: BLOCKBOT_PROJECT_ID,
            Key: {
                'sender' : {S: getMessageSender()},
            }
        };

        ddb.getItem(params, function(err, result) {
            if (err) {
                // TODO handle this?
            } else {
                data = result;
                sync = false;
            }
        });
        while(sync) {deasync.sleep(100);}

        return data.Item[variableName]["S"];
    }

    //
    // Calendar helper functions
    //
    function getCalendarName(calendar) {
        return calendar.name;
    }

    function createCalendarEvent(calendar, title, startTime, durationInMinutes) {
        var sync = true;
        request.post({
            url: 'https://blockbot.io/GoogleProxy/CreateCalendarEvent',
            form:
                {
                    username: BLOCKBOT_NORMALIZED_USERNAME,
                    calendarId: calendar.id,
                    title: title,
                    startYear: startTime.getFullYear(),
                    startMonth: startTime.getMonth() + 1,
                    startDay: startTime.getDate(),
                    startHour: startTime.getHours(),
                    startMinute: startTime.getMinutes(),
                    durationInMinutes: durationInMinutes
                },
            rejectUnauthorized: false
        }, function(err,httpResponse,body){
            sync = false;
        });
        while(sync) {deasync.sleep(100);}
    }

    // function getNextNCalendarEvents(calendar, n){
    //     var startTime = new Date();
    //     request.post({
    //         url: 'https://localhost:44305/GoogleProxy/GetNextNCalendarEvents',
    //         form:
    //             {
    //                 username: BLOCKBOT_NORMALIZED_USERNAME,
    //                 calendarId: calendar,
    //                 startYear: startTime.getFullYear(),
    //                 startMonth: startTime.getMonth() + 1,
    //                 startDay: startTime.getDate(),
    //                 startHour: startTime.getHours(),
    //                 startMinute: startTime.getMinutes(),
    //                 n: n
    //             },
    //         rejectUnauthorized: false
    //     });
    // }

    function getNextAvailableCalendarEvent(calendar, durationInMinutes) {
        var sync = true;
        var data = null;
        var startTime = new Date();
        request.post({
            url: 'https://blockbot.io/GoogleProxy/GetNextNAvailableCalendarEventSlots',
            form:
                {
                    username: BLOCKBOT_NORMALIZED_USERNAME,
                    calendarId: calendar.id,
                    startYear: startTime.getFullYear(),
                    startMonth: startTime.getMonth() + 1,
                    startDay: startTime.getDate(),
                    startHour: startTime.getHours(),
                    startMinute: startTime.getMinutes(),
                    n: 1,
                    durationInMinutes: durationInMinutes
                },
            rejectUnauthorized: false
        }, function(err,httpResponse,body){
            data = body;
            sync = false;
        });
        while(sync) {deasync.sleep(100);}

        var month = parseInt(data.slice(6, 8), 10);
        if (month === 1) {
            month = 12;
        }
        var hour = parseInt(data.slice(12, 14), 10);
        if (hour === 1){
            hour = 24;
        }
        return new Date(
            data.slice(1, 5),
            month - 1,
            data.slice(9, 11),
            hour, // handle time zone
            data.slice(15, 17),
            0,
            0);
    }


    //
    // placeholder for javascript from workspace
    //
    // START_CODE_PLACEHOLDER


    // END_CODE_PLACEHOLDER

    sendMessage();
};
