let tag = "#tgTag";

function checkTgTag(tag) {
    let value = $(tag).val();
    let isValid = value.startsWith('@') && value.length >= 6;
    let warn = "#validateTgTag";
    if (!isValid) {
        $(warn).html('Тег повинен починатися з символа @<br>та містити не менш ніж 6 символів, враховуючи @');
    }
    else {
        $(warn).text('');
    }
    return isValid;
}

$(tag).keyup(function () {
    checkTgTag(this);
});

let form = '#changeTgTagForm';
$(form).submit(function (event) {
    if (!checkTgTag(tag))
        event.preventDefault();
});