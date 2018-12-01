'use strict';

const PresentOptions = require('./PresentOptions');

module.exports.handler = (event, context, callback) => {
    const twilio_request = {}
    const body_array = event.body.split('&')
    for (var i = 0; i < body_array.length; i++){
        var x = body_array[i].split('=')
        twilio_request[x[0]] = x.length > 1 ? decodeURIComponent(x[1]) : null
    }
    console.log(twilio_request)

    // TODO look up API key by account sid
    const TWILIO_API_KEY="667b624751c4fd22e9a5c5b1bd2b76fe"

    var response_message = `<?xml version="1.0" encoding="UTF-8" ?><Response>`


    //
    // Message functions
    //
    function addMessage(messageText){
        response_message += "<Message>" + messageText + "</Message>";
    }

    function getMessageBody(){
        // TODO return text of message
    }

    function getMessageSender(){
        // TODO return sender of message
    }

    //
    // Control flow helper functions
    //
    function startConversation(){
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
    function isOptionSelectionUnseen(optionPrompt){

    }

    function isOptionSelectionPending(optionPrompt){

    }

    function setOptionSelectionPending(optionPrompt){

    }

    function isOptionSelectionSelected(optionPrompt){
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

        if (stylistSet === false){
            // TODO set variable in DynamoDb
        }
    }

    function getVariable(variableName) {

    }

    //
    // Calendar helper functions
    //
    function getCalendarLink(calendarName){
        // TODO fetch calendar link/key based on calendar name
    }


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
    var opt = new PresentOptions(optionPrompt);

    if (isOptionSelectionUnseen(optionPrompt)) {
        setOptionSelectionPending(optionPrompt);
        addMessage('Hair Design, Inc. Choose a number.%0a1 - appt with Nicole%0a2 - appt with Michelle%0a3 - see stores hours');
        sendMessage();
    } else {
        var switchValue;
        if (isOptionSelectionPending(optionPrompt)) {
            switchValue = getMessageBody().replace(/\D/g,'');
        } else {
            switchValue = getOptionSelected(optionPrompt);
        }
        // TODO explore if this will allow nested "present option" blocks
        if (isOptionSelectionPending(optionPrompt)){

            switch (switchValue) {
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








    // END_CODE_PLACEHOLDER
    response_message += '</Response>'

    callback(null, {
        statusCode: 200,
        headers: {"content-type": "application/xml"},
        body: response_message
    })
};
