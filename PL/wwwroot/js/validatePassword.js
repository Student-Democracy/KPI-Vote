let newPassword = '#NewPassword';

function checkConfirmation() {
    let confirm = '#NewPasswordConfirm';
    let confirmationErrorBox = '#confirm-new-pswd-warn';
    if ($(newPassword).val() != $(confirm).val()) {
        $(confirmationErrorBox).text('Новий пароль та його підтвердження відрізняються');
        return false;
    }
    else {
        $(confirmationErrorBox).text('');
        return true;
    };
};

$(confirm).keyup(function () {
    checkConfirmation();
});

let general = '#general-validate';
let chars = '#8chars';
let upper = '#1upper';
let lower = '#1lower';
let number = '#1number';

function checkPassword() {
    let password = $(newPassword).val();
    let isChars = false;
    let isUpper = false;
    let isLower = false;
    let isNumber = false;
    if (password.length >= 8) {
        $(chars).css('color', 'green');
        isChars = true;
    }
    else {
        $(chars).css('color', 'red');
    }
    if (password.match('^(?=.*[A-Z])')) {
        $(upper).css('color', 'green');
        isUpper = true;
    }
    else {
        $(upper).css('color', 'red');
    }
    if (password.match('^(?=.*[a-z])')) {
        $(lower).css('color', 'green');
        isLower = true;
    }
    else {
        $(lower).css('color', 'red');
    }
    if (password.match('^(?=.*\\d)')) {
        $(number).css('color', 'green');
        isNumber = true;
    }
    else {
        $(number).css('color', 'red');
    }
    if (isChars && isNumber && isUpper && isLower) {
        $(general).css('color', 'green');
        return true;
    }
    else {
        $(general).css('color', 'red');
        return false;
    }
}

$(newPassword).keyup(function () {
    checkConfirmation();
    checkPassword();
});

let form = '#change-pswd';
$(form).submit(function (event) {
    if (!checkConfirmation() || !checkPassword())
        event.preventDefault();
});