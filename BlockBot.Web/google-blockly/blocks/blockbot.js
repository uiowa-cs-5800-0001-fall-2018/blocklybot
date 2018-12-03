goog.require('Blockly.Blocks');
goog.require('Blockly');

Blockly.defineBlocksWithJsonArray(
    [{
      "type": "start_conversation",
      "lastDummyAlign0": "CENTRE",
      "message0": "Start conversation",
      "nextStatement": null,
      "colour": 230,
      "tooltip": "This block runs when a message is received from the user.",
      "helpUrl": ""
    },
    {
      "type": "end_conversation",
      "message0": "End conversation",
      "previousStatement": null,
      "colour": 230,
      "tooltip": "This block marks a conversation as complete. Future messages from this sender will be treated like a new conversation.",
      "helpUrl": ""
    },
    {
      "type": "send_message",
      "message0": "Send Message %1",
      "args0": [
        {
          "type": "input_value",
          "name": "message_text",
          "check": "String"
        }
      ],
      "previousStatement": null,
      "nextStatement": null,
      "colour": 230,
      "tooltip": "Send a message to the user",
      "helpUrl": ""
    },
    {
      "type": "get_message_body",
      "lastDummyAlign0": "RIGHT",
      "message0": "Message Text",
      "output": "String",
      "colour": 230,
      "tooltip": "Gets the text of the message that was received",
      "helpUrl": ""
    },
    {
      "type": "get_message_sender",
      "message0": "Message Sender",
      "output": "String",
      "colour": 230,
      "tooltip": "Get the sender from the message that was received",
      "helpUrl": ""
    },
    {
      "type": "switch",
      "message0": "Switch %1 Input %2 Cases %3 Default %4",
      "args0": [
        {
          "type": "input_dummy"
        },
        {
          "type": "input_value",
          "name": "Input",
          "align": "RIGHT"
        },
        {
          "type": "input_statement",
          "name": "Cases",
          "check": "cases",
          "align": "RIGHT"
        },
        {
          "type": "input_statement",
          "name": "Default",
          "align": "RIGHT"
        }
      ],
      "previousStatement": null,
      "nextStatement": null,
      "colour": 230,
      "tooltip": "TODO",
      "helpUrl": ""
    },
    {
      "type": "case",
      "message0": "Case %1 %2",
      "args0": [
        {
          "type": "input_value",
          "name": "case",
          "align": "RIGHT"
        },
        {
          "type": "input_statement",
          "name": "case_body"
        }
      ],
      "previousStatement": [
        "case",
        "cases"
      ],
      "nextStatement": "case",
      "colour": 230,
      "tooltip": "",
      "helpUrl": ""
    },








    {
      "type": "options",
      "message0": "Prompt %1 Present Options %2",
      "args0": [
        {
          "type": "input_value",
          "name": "Prompt",
          "align": "RIGHT"
        },
        {
          "type": "input_statement",
          "name": "Options"
        }
      ],
      "previousStatement": null,
      "nextStatement": null,
      "colour": 230,
      "tooltip": "Present a list of options to the user and let them select one",
      "helpUrl": ""
    },
    {
      "type": "option",
      "message0": "Option %1 Do %2",
      "args0": [
        {
          "type": "input_value",
          "name": "Option",
          "check": "String"
        },
        {
          "type": "input_statement",
          "name": "Statements",
          "align": "RIGHT"
        }
      ],
      "previousStatement": null,
      "nextStatement": null,
      "colour": 230,
      "tooltip": "",
      "helpUrl": ""
    },
    {
      "type": "options_alternative",
      "message0": "Prompt %1 Present Options %2 Alternative %3",
      "args0": [
        {
          "type": "input_value",
          "name": "Prompt",
          "check": "String",
          "align": "RIGHT"
        },
        {
          "type": "input_statement",
          "name": "Options",
          "check": "option",
          "align": "RIGHT"
        },
        {
          "type": "input_statement",
          "name": "Alternative",
          "align": "RIGHT"
        }
      ],
      "previousStatement": null,
      "nextStatement": null,
      "colour": 230,
      "tooltip": "Present a list of options to the user and let them select one. End with an alternative input prompt. If the user does not select an option, the alternative block will be run.",
      "helpUrl": ""
    },










    {
      "type": "calendar_create_appt",
      "lastDummyAlign0": "RIGHT",
      "message0": "Create Calendar Event %1 Calendar %2 Title %3 Start Time %4 Duration %5",
      "args0": [
        {
          "type": "input_dummy"
        },
        {
          "type": "input_value",
          "name": "CALENDAR",
          "align": "RIGHT"
        },
        {
          "type": "input_value",
          "name": "TITLE",
          "align": "RIGHT"
        },
        {
          "type": "input_value",
          "name": "START_TIME",
          "align": "RIGHT"
        },
        {
          "type": "input_value",
          "name": "DURATION",
          "check": "Number",
          "align": "RIGHT"
        }
      ],
      "inputsInline": false,
      "previousStatement": null,
      "nextStatement": null,
      "colour": 230,
      "tooltip": "",
      "helpUrl": ""
    },
    {
      "type": "calendar_calendar",
      "message0": "Calendar %1 (id %2)",
      "args0": [
        {
          "type": "field_input",
          "name": "NAME",
          "text": "default"
        },
        {
          "type": "field_input",
          "name": "ID",
          "text": "default",
        }
      ],
      "output": "calendar",
      "colour": 230,
      "tooltip": "",
      "helpUrl": ""
    },
    {
      "type": "calendar_next_available",
      "message0": "Next available time(s) %1 Number of times %2 %3 Calendar %4",
      "args0": [
        {
          "type": "input_dummy"
        },
        {
          "type": "field_dropdown",
          "name": "TIMES",
          "options": [
            [
              "1",
              "1"
            ],
            [
              "2",
              "2"
            ],
            [
              "3",
              "3"
            ]
          ]
        },
        {
          "type": "input_dummy",
          "align": "RIGHT"
        },
        {
          "type": "input_value",
          "name": "CALENDAR",
          "check": "String",
          "align": "RIGHT"
        }
      ],
      "output": null,
      "colour": 230,
      "tooltip": "",
      "helpUrl": ""
    },
    {
      "type": "datetime",
      "message0": "",
      "output": null,
      "colour": 230,
      "tooltip": "",
      "helpUrl": ""
    },
    {
      "type": "calendar_get_name",
      "message0": "Get Calendar Name %1",
      "args0": [
        {
          "type": "input_value",
          "name": "CALENDAR",
          "check": "calendar"
        }
      ],
      "inputsInline": false,
      "output": "String",
      "colour": 230,
      "tooltip": "Gets the name of a specific calendar.",
      "helpUrl": ""
    }]);
