// Общие скрипты сайта

// Подтверждение для форм с data-confirm
document.addEventListener('submit', function (event) {
    const form = event.target.closest('form[data-confirm]');
    if (form && !window.confirm(form.dataset.confirm)) {
        event.preventDefault();
    }
});

// Отправка формы сразу после выбора значения в select с data-autosubmit
document.addEventListener('change', function (event) {
    const select = event.target.closest('select[data-autosubmit]');
    if (select && select.value && select.form) {
        select.form.requestSubmit ? select.form.requestSubmit() : select.form.submit();
    }
});
