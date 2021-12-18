function setChars(textareas) {
    let chars = $(textareas).val().trim().length;
    let element = $('#desc-characters');
    element.text(chars);
    let color;
    if (chars >= 1000) {
        color = 'green';
    }
    else {
        color = 'red';
    }
    element.css('color', color);
};

function validateFloat(float, min, max) {
    console.log(1);
    let value = $(float).val();
    if (value != '') {
        console.log(value);
        for (let i = 0; i < value.length; i++) {
            console.log(value.charAt(i));
            if (value.charAt(i) != ',' && (value.charAt(i) < '0' || value.charAt(i) > '9'))
                return false;
            console.log(value.charAt(i));
        }
        let parsed = parseFloat(value);
        console.log(parsed);
        if (parsed != null && parsed >= min && parsed <= max)
            return true;
    }
    return false;
};

function drawOutline(float, result) {
    if (result) {
        $(float).css('outline', 'none');
    }
    else {
        $(float).css('outline', '1px solid red');
    }
}

autosize($('#description'));
setChars('#description');
$('#description').keyup(function () {
    setChars(this);
});

let attendance = '#MinimalAttendancePercentage';
let minAttendance = 1;
let max = 100;

$(attendance).keyup(function () {
    drawOutline(this, validateFloat(this, minAttendance, max));
});

let minFor = 50;
let forPercentage = '#MinimalForPercentage';
$(forPercentage).keyup(function () {
    drawOutline(this, validateFloat(this, minFor, max));
});

$('#add-voting').submit(function (event) {
    let chars = $('#description').val().trim().length;
    if (chars < 1000) {
        event.preventDefault();
    }
    if (!validateFloat(attendance, minAttendance, max)) {
        event.preventDefault();
        $(attendance).css('outline', '1px solid red');
    }
    if (!validateFloat(forPercentage, minFor, max)) {
        event.preventDefault();
        $(forPercentage).css('outline', '1px solid red');
    }
})