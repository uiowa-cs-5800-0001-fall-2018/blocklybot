
Blockly.JavaScript['send_sms'] = function(block) {
    var value_message_text = Blockly.JavaScript.valueToCode(block, 'message_text', Blockly.JavaScript.ORDER_ATOMIC);
    // TODO: Assemble JavaScript into code variable.
    var code = 'response_message += \'<Message>' + value_message_text.slice(1, value_message_text.length - 1) + '</Message>\'\n';
    return code;
};

Blockly.JavaScript['receive_sms'] = function(block) {
    var code = ''; // does nothing - handled by lambda function
    return code;
};

Blockly.JavaScript['get_sms_body'] = function(block) {
    var code = 'twilio_request.Body';
    // TODO: Change ORDER_NONE to the correct strength.
    return [code, Blockly.JavaScript.ORDER_NONE];
};

Blockly.JavaScript['get_sms_sender'] = function(block) {
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