
Blockly.JavaScript['name_block'] = function (block) {
    var text_question = block.getFieldValue('question');
    var value_if = Blockly.JavaScript.valueToCode(block, 'if', Blockly.JavaScript.ORDER_ATOMIC);
    var statements_then = Blockly.JavaScript.statementToCode(block, 'then');
    var statements_else = Blockly.JavaScript.statementToCode(block, 'else');
    // TODO: Assemble JavaScript into code variable.
    var code = '...;\n';
    return code;
};