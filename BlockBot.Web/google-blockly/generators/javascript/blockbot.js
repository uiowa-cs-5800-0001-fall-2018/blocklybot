Blockly.JavaScript['start_conversation'] = function(block) {
    var code = 'startConversation();\n';
    return code;
};

Blockly.JavaScript['end_conversation'] = function(block) {
    var code = 'endConversation();\n'; // TODO log end of conversation in DynamoDB
    return code;
};

Blockly.JavaScript['send_message'] = function(block) {
    var message_text = Blockly.JavaScript.valueToCode(block, 'message_text', Blockly.JavaScript.ORDER_ATOMIC);
    return "addMessage(" + message_text + ");\n";
};



Blockly.JavaScript['get_message_body'] = function(block) {
    var code = 'getMessageBody()';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['get_message_sender'] = function(block) {
    var code = 'getMessageSender()';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};


// Blockly.JavaScript['switch'] = function(block) {
//     var value_input = Blockly.JavaScript.valueToCode(block, 'Input', Blockly.JavaScript.ORDER_ATOMIC);
//     var statements_cases = Blockly.JavaScript.statementToCode(block, 'Cases');
//     var statements_default = Blockly.JavaScript.statementToCode(block, 'Default');
//     var code = 'switch (' + value_input + ') {\n' + statements_cases + 'default:\n' + statements_default + '}\n';
//     return code;
// };
//
// Blockly.JavaScript['case'] = function(block) {
//     var value_case = Blockly.JavaScript.valueToCode(block, 'case', Blockly.JavaScript.ORDER_ATOMIC);
//     var statements_case_body = Blockly.JavaScript.statementToCode(block, 'case_body');
//     var code = 'case ' + value_case + ':\n' + statements_case_body + 'break;\n';
//     return code;
// };




Blockly.JavaScript['options'] = function(block) {
    var value_prompt = Blockly.JavaScript.valueToCode(block, 'Prompt', Blockly.JavaScript.ORDER_ATOMIC);

    var codeArray = [];
    var optionArray = [];
    var currentBlock = this.getInputTargetBlock('Options');
    while (currentBlock) {
        var codeForBlock = Blockly.JavaScript.singleBlockToCode(currentBlock);
        codeArray.push(codeForBlock);
        var optionForBlock = Blockly.JavaScript.valueToCode(currentBlock, 'Option', Blockly.JavaScript.ORDER_ATOMIC);
        optionArray.push(optionForBlock);
        currentBlock = currentBlock.getNextBlock();
    }

    var code_string = "";
    var option_string = "";
    var regex = /case-idx/g;
    for (let i = 0; i < codeArray.length; i++) {
        code_string += codeArray[i].replace(regex, (i + 1).toString());
        option_string += "\n" + (i + 1).toString() + " - " + optionArray[i].slice(1, optionArray[i].length - 1);
    }

    return "    optionPrompt = " + value_prompt +";\n" +
        "    if (isOptionSelectionUnseen(optionPrompt)) {\n" +
        "        setOptionSelectionPending(optionPrompt);\n" +
        "        addMessage(optionPrompt + '" + option_string + "');\n" +
        "        sendMessage();\n" +
        "    } else {\n" +
        "        if (isOptionSelectionPending(optionPrompt)){\n" +
        "            switch (getMessageBody().replace(/\\D/g,'')) {\n" +
                    code_string +
        "                default:\n" +
        "                    addMessage('Unable to interpret response. Please select one of the provided options.');\n" +
        "                    sendMessage();\n" +
        "                    break;\n" +
        "            }\n" +
        "        }\n" +
        "        \n" +
        "    }";
};

Blockly.JavaScript['options_alternative'] = function(block) {
    var statements_alternative = Blockly.JavaScript.statementToCode(block, 'Alternative');


    var value_prompt = Blockly.JavaScript.valueToCode(block, 'Prompt', Blockly.JavaScript.ORDER_ATOMIC);

    var codeArray = [];
    var optionArray = [];
    var currentBlock = this.getInputTargetBlock('Options');
    while (currentBlock) {
        var codeForBlock = Blockly.JavaScript.singleBlockToCode(currentBlock);
        codeArray.push(codeForBlock);
        var optionForBlock = Blockly.JavaScript.valueToCode(currentBlock, 'Option', Blockly.JavaScript.ORDER_ATOMIC);
        optionArray.push(optionForBlock);
        currentBlock = currentBlock.getNextBlock();
    }

    var code_string = "";
    var option_string = "";
    var regex = /case-idx/g;
    for (let i = 0; i < codeArray.length; i++) {
        code_string += codeArray[i].replace(regex, (i + 1).toString());
        option_string += "%0a" + (i + 1).toString() + " - " + optionArray[i].slice(1, optionArray[i].length - 1);
    }

    return "    optionPrompt = " + value_prompt +";\n" +
        "    if (isOptionSelectionUnseen(optionPrompt)) {\n" +
        "        setOptionSelectionPending(optionPrompt);\n" +
        "        addMessage(optionPrompt + '" + option_string + "');\n" +
        "        sendMessage();\n" +
        "    } else {\n" +
        "        if (isOptionSelectionPending(optionPrompt)){\n" +
        "            switch (getMessageBody().replace(/\\D/g,'')) {\n" +
         code_string +
        "                default:\n" +
          statements_alternative +
        "                    sendMessage();\n" +
        "                    break;\n" +
        "            }\n" +
        "        }\n" +
        "        \n" +
        "    }";
};

Blockly.JavaScript['option'] = function(block) {
    //var value_option = Blockly.JavaScript.valueToCode(block, 'Option', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_statements = Blockly.JavaScript.statementToCode(block, 'Statements');
    // TODO caseFinished(optionPrompt, "case-idx") function at end
    return "case \"case-idx\":\nsetOptionSelectionSelected(optionPrompt, \"case-idx\");\n" + statements_statements + "break;\n";
};




Blockly.JavaScript['calendar_create_appt'] = function(block) {
    var value_calendar = Blockly.JavaScript.valueToCode(block, 'CALENDAR', Blockly.JavaScript.ORDER_ATOMIC);
    var value_title = Blockly.JavaScript.valueToCode(block, 'TITLE', Blockly.JavaScript.ORDER_ATOMIC);
    var value_start_time = Blockly.JavaScript.valueToCode(block, 'START_TIME', Blockly.JavaScript.ORDER_ATOMIC);
    var value_duration = Blockly.JavaScript.valueToCode(block, 'DURATION', Blockly.JavaScript.ORDER_ATOMIC);
    return 'createCalendarEvent(' + value_calendar + ', ' + value_title
        + ', ' + value_start_time + ', ' + value_duration + ');\n';
};

Blockly.JavaScript['calendar_calendar'] = function(block) {
    var dropdown_name = block.getFieldValue('NAME');
    var value_id = block.getFieldValue('ID');
    var code = 'new Calendar("' + dropdown_name +'", "' + value_id + '")';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['calendar_next_available'] = function(block) {
    var value_duration = Blockly.JavaScript.valueToCode(block, 'DURATION', Blockly.JavaScript.ORDER_ATOMIC);
    var value_calendar = Blockly.JavaScript.valueToCode(block, 'CALENDAR', Blockly.JavaScript.ORDER_ATOMIC);
    // TODO: Assemble JavaScript into code variable.
    var code = 'getNextAvailableCalendarEvent(' + value_calendar + ', ' + value_duration + ')';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

// Blockly.JavaScript['datetime'] = function(block) {
//     // TODO: Assemble JavaScript into code variable.
//     var code = '...';
//     // TODO: Change ORDER_NONE to the correct strength.
//     return [code, Blockly.JavaScript.ORDER_NONE];
// };

Blockly.JavaScript['calendar_get_name'] = function(block) {
    var value_calendar = Blockly.JavaScript.valueToCode(block, 'CALENDAR', Blockly.JavaScript.ORDER_ATOMIC);
    // TODO: Assemble JavaScript into code variable.
    var code = 'getCalendarName(' + value_calendar + ')';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};