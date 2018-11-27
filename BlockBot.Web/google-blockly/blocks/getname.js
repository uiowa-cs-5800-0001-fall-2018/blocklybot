goog.require('Blockly.Blocks');
goog.require('Blockly');

Blockly.defineBlocksWithJsonArray(
[
        {
            "type": "name_block",
            "message0": "Send SMS %1 %2 if %3 then %4 else %5",
            "args0": [
                {
                    "type": "field_input",
                    "name": "question",
                    "text": "Hello! Can you please tell me your first and last name?"
                },
                {
                    "type": "input_dummy"
                },
                {
                    "type": "input_value",
                    "name": "if"
                },
                {
                    "type": "input_statement",
                    "name": "then"
                },
                {
                    "type": "input_statement",
                    "name": "else"
                }
            ],
            "previousStatement": null,
            "nextStatement": null,
            "colour": 230,
            "tooltip": "Ask for user first and last name",
            "helpUrl": ""
        }
]);