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
    var code = 'twilio_request.Body';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['get_message_sender'] = function(block) {
    var code = 'twilio_request.From';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};


Blockly.JavaScript['switch'] = function(block) {
    var value_input = Blockly.JavaScript.valueToCode(block, 'Input', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_cases = Blockly.JavaScript.statementToCode(block, 'Cases');
    var statements_default = Blockly.JavaScript.statementToCode(block, 'Default');
    var code = 'switch (' + value_input + ') {\n' + statements_cases + 'default:\n' + statements_default + '}\n';
    return code;
};

Blockly.JavaScript['case'] = function(block) {
    var value_case = Blockly.JavaScript.valueToCode(block, 'case', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_case_body = Blockly.JavaScript.statementToCode(block, 'case_body');
    var code = 'case ' + value_case + ':\n' + statements_case_body + 'break;\n';
    return code;
};




Blockly.JavaScript['options'] = function(block) {
    var value_prompt = Blockly.JavaScript.valueToCode(block, 'Prompt', Blockly.JavaScript.ORDER_ATOMIC);
    value_prompt = value_prompt.slice(1, value_prompt.length - 1); // remove quotes

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

    var code1 = "optionPrompt = \"TODO-option-prompt\";\n" +
        "    if (isOptionSelected(optionPrompt) === false) {\n" +
        "        // present options\n" +
        "        addMessage('Hair Design, Inc. Choose a number.%0a1 - appt with Nicole%0a2 - appt with Michelle%0a3 - see stores hours');\n" +
        "        sendMessage();\n" +
        "    } else {\n" +
        "        // todo strip body\n" +
        "        switch (twilio_request.Body) {\n" +

        "            default:\n" +
        "                addMessage('Unable to interpret response. Please select one of the provided options.');\n" +
        "                sendMessage();\n" +
        "                break;\n" +
        "        }\n" +
        "    }"

    var code = "optionPrompt = " + value_prompt + ";\n" +
        "if (isOptionSelected(optionPrompt) === false) {\n" +
        "    addMessage('" + value_prompt + option_string + "');\n" +
        "    sendMessage();\n" +
        "} else {\n" +
        "    switch (twilio_request.Body.replace(/\\D/g,'')) {\n" +
        code_string +
        "        default:\n" +
        "            addMessage('Unable to interpret response. Please select one of the provided options.');\n" +
        "            sendMessage();\n" +
        "            break;\n" +
        "    }\n" +
        "}"

    return code;
};

Blockly.JavaScript['options_alternative'] = function(block) {
    var value_name = Blockly.JavaScript.valueToCode(block, 'NAME', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_options = Blockly.JavaScript.statementToCode(block, 'options');
    var statements_alternative = Blockly.JavaScript.statementToCode(block, 'alternative');
    // TODO: Assemble JavaScript into code variable.
    var code = '...;\n';
    return code;
};

Blockly.JavaScript['option'] = function(block) {
    var value_option = Blockly.JavaScript.valueToCode(block, 'Option', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_statements = Blockly.JavaScript.statementToCode(block, 'Statements');
    // TODO: Assemble JavaScript into code variable.
    var code = "case \"case-idx\":\nsetOptionSelected(optionPrompt, \"case-idx\");\n" + statements_statements + "break;\n"
    return code;
};




Blockly.JavaScript['calendar_create_appt'] = function(block) {
    var value_calendar = Blockly.JavaScript.valueToCode(block, 'CALENDAR', Blockly.JavaScript.ORDER_ATOMIC);
    var value_title = Blockly.JavaScript.valueToCode(block, 'TITLE', Blockly.JavaScript.ORDER_ATOMIC);
    var value_start_time = Blockly.JavaScript.valueToCode(block, 'START_TIME', Blockly.JavaScript.ORDER_ATOMIC);
    var dropdown_duration = block.getFieldValue('Duration');
    // TODO: Assemble JavaScript into code variable.
    var code = '...;\n';
    return code;
};

Blockly.JavaScript['calendar_calendar'] = function(block) {
    var dropdown_name = block.getFieldValue('NAME');
    // TODO: Assemble JavaScript into code variable.
    var code = '...';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['calendar_next_available'] = function(block) {
    var dropdown_times = block.getFieldValue('TIMES');
    var value_calendar = Blockly.JavaScript.valueToCode(block, 'CALENDAR', Blockly.JavaScript.ORDER_ATOMIC);
    // TODO: Assemble JavaScript into code variable.
    var code = '...';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['datetime'] = function(block) {
    // TODO: Assemble JavaScript into code variable.
    var code = '...';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['calendar_get_name'] = function(block) {
    var value_calendar = Blockly.JavaScript.valueToCode(block, 'CALENDAR', Blockly.JavaScript.ORDER_ATOMIC);
    // TODO: Assemble JavaScript into code variable.
    var code = '...';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};