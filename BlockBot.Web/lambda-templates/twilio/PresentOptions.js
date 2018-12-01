class PresentOptions {
    constructor(optionPrompt) {
        this.optionPrompt = optionPrompt;
    }
    get optionPrompt() {
        return this.optionPrompt;
    }
    sayHello() {
        console.log('Hello, my name is ' + this.name + ', I have ID: ' + this.id);
    }
}

module.exports = PresentOptions;