Blockly.JavaScript['start_conversation'] = function(block) {
    var code = '...;\n'; // TODO log conversation in DynamoDB
    return code;
};

Blockly.JavaScript['end_conversation'] = function(block) {
    var code = '...;\n'; // TODO log end of conversation in DynamoDB
    return code;
};

Blockly.JavaScript['send_message'] = function(block) {
    var value_message_text = Blockly.JavaScript.valueToCode(block, 'message_text', Blockly.JavaScript.ORDER_ATOMIC);
    // TODO: Assemble JavaScript into code variable.
    var code = 'response_message += \'<Message>' + value_message_text.slice(1, value_message_text.length - 1) + '</Message>\'\n';
    return code;
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
    var value_name = Blockly.JavaScript.valueToCode(block, 'NAME', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_name = Blockly.JavaScript.statementToCode(block, 'NAME');
    // TODO: Assemble JavaScript into code variable.
    var code = '...;\n';
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
    var value_name = Blockly.JavaScript.valueToCode(block, 'NAME', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_name = Blockly.JavaScript.statementToCode(block, 'NAME');
    // TODO: Assemble JavaScript into code variable.
    var code = '...;\n';
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