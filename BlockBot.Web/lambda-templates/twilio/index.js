'use strict';
const PresentOptions = require('./PresentOptions');
const Calendar = require('./Calendar');

const {google} = require('googleapis');

var request = require('request');

const scopes = [
    'https://www.googleapis.com/auth/userinfo.profile',
    'https://www.googleapis.com/auth/calendar.readonly',
    'https://www.googleapis.com/auth/calendar.events'
];

module.exports.handler = (event, context, callback) => {
    // const twilio_request = {}
    // const body_array = event.body.split('&')
    // for (var i = 0; i < body_array.length; i++){
    //     var x = body_array[i].split('=')
    //     twilio_request[x[0]] = x.length > 1 ? decodeURIComponent(x[1]) : null
    // }
    // console.log(twilio_request)

    // TODO look up API key by account sid
    const TWILIO_API_KEY = "667b624751c4fd22e9a5c5b1bd2b76fe";
    const GOOGLE_CLIENT_ID = "1064809067576-ik4i954ifh322bk4v2a61fc2o80cpq7d.apps.googleusercontent.com";
    const GOOGLE_CLIENT_SECRET = "YiMhs7jy0UjXOVUH47KErMPFE";
    const GOOGLE_REFRESH_TOKEN = "1/IqU7hZon1hI_cZRv6fCiDG6qjIoDLQglFF8pmmIjT7g";
    const BLOCKBOT_NORMALIZED_USERNAME = "HARLEY@WALDSTEIN.IO";
    const BLOCKBOT_PROJECT_ID = "";

    var response_message = `<?xml version="1.0" encoding="UTF-8" ?><Response>`


    //
    // Message functions
    //
    function addMessage(messageText) {
        response_message += "<Message>" + messageText + "</Message>";
    }

    function getMessageBody() {
        // twilio_request.Body
        // TODO return text of message
    }

    function getMessageSender() {
        // twilio_request.From
        // TODO return sender of message
    }

    //
    // Control flow helper functions
    //
    function startConversation() {
        var phoneNumber = getMessageSender();
        // TODO check if open conversation exists in dynamodb
        // TODO if not, register new conversation
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
        // TODO mark conversation as finished in dynamodb

        sendMessage();
    }


    //
    // Option helper functions: Unseen -> Pending -> Selected
    //
    function isOptionSelectionUnseen(optionPrompt) {

    }

    function isOptionSelectionPending(optionPrompt) {

    }

    function setOptionSelectionPending(optionPrompt) {

    }

    function isOptionSelectionSelected(optionPrompt) {
        // TODO return true if option is selected, otherwise false
    }

    function setOptionSelectionSelected(optionPrompt, optionValue) {
        // TODO set that the given value was selected for the given prompt
    }

    function getOptionSelected(optionPrompt) {
        // TODO return value set for optionPrompt
    }

    //
    // Variable helper functions
    //
    function setVariable(variableName, variableValue) {
        // TODO check to see if value is set in DynamoDb
        var stylistSet = false;

        if (stylistSet === false) {
            // TODO set variable in DynamoDb
        }
    }

    function getVariable(variableName) {

    }

    //
    // Calendar helper functions
    //
    function getCalendarName(calendar) {
        return calendar.name;
    }

    function createCalendarEvent(calendar, title, startTime, durationInMinutes) {
        request.post({
            url: 'https://localhost:44305/GoogleProxy/CreateCalendarEvent',
            form:
                {
                    username: BLOCKBOT_NORMALIZED_USERNAME,
                    calendarId: calendar,
                    title: title,
                    startYear: startTime.getFullYear(),
                    startMonth: startTime.getMonth() + 1,
                    startDay: startTime.getDate(),
                    startHour: startTime.getHours(),
                    startMinute: startTime.getMinutes(),
                    durationInMinutes: durationInMinutes
                },
            rejectUnauthorized: false
        });
    }


    createCalendarEvent("harley@waldstein.io", "test 2", new Date(2018, 11, 3, 11, 0, 0, 0), 30);
    return;

    //
    // Helper variables
    //
    var optionPrompt = "";

    //
    // placeholder for javascript from workspace
    //
    // START_CODE_PLACEHOLDER

    startConversation();

    // message initially received, present options

    optionPrompt = "Hair Design, Inc. Choose a number.";
    if (isOptionSelectionUnseen(optionPrompt)) {
        setOptionSelectionPending(optionPrompt);
        addMessage('Hair Design, Inc. Choose a number.%0a1 - appt with Nicole%0a2 - appt with Michelle%0a3 - see stores hours');
        sendMessage();
    } else {
        // TODO explore if this will allow nested "present option" blocks
        if (isOptionSelectionPending(optionPrompt)) {

            switch (getMessageBody().replace(/\D/g, '')) {
                case "case-idx":
                    setOptionSelectionSelected(optionPrompt, "case-idx");

                    setVariable("stylist", getCalendarLink("TODO-calendar-Nicole"));

                    // TODO could include function reference here -- figure out how that works

                    break;
                case "case-idx":
                    setOptionSelectionSelected(optionPrompt, "case-idx");

                    // set stylist
                    setVariable("stylist", getCalendarLink("TODO-calendar-Michelle"));

                    // TODO could include function reference here -- figure out how that works

                    break;
                case "case-idx":
                    setOptionSelectionSelected(optionPrompt, "case-idx");

                    addMessage('Store hours are M-F 8am-5pm, closed weekends.');

                    endConversation();

                    break;
                default:
                    addMessage('Unable to interpret response. Please select one of the provided options.');
                    sendMessage();
                    break;
            }
        }

    }

    var c = new Calendar("", "");
    c.// END_CODE_PLACEHOLDER
        response_message += '</Response>'

    callback(null, {
        statusCode: 200,
        headers: {"content-type": "application/xml"},
        body: response_message
    })
};
