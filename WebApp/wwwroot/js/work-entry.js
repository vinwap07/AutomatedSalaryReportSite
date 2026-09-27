// Форма записи о работе (.work-entry) подстраивается под специальность выбранного работника:
// у разнорабочего нет техники и часов, свои единицы измерения и оплата окладом,
// а виды работ специальности работника поднимаются в начало списка.
// Видимость полей задается классами (см. site.css), скрытые поля отключаются и не отправляются с формой
window.WorkEntry = (function () {
    const LABORER = 'Laborer';
    const SALARY = 'Salary';
    const TOGGLED = '.specialist-only, .laborer-only, .hide-for-salary, .salary-only';

    function isShown(element, isLaborer, isSalary) {
        return !(element.matches('.specialist-only') && isLaborer)
            && !(element.matches('.laborer-only') && !isLaborer)
            && !(element.matches('.hide-for-salary') && isSalary)
            && !(element.matches('.salary-only') && !isSalary);
    }

    // Группа специальности работника — первой, затем неназначенные работы, затем остальные по порядку
    function sortWorkTypes(select, specialty) {
        if (!select) return;
        const value = select.value;
        const rank = function (group) {
            if (specialty && group.dataset.specialty === specialty) return -2;
            if (group.dataset.specialty === '') return -1;
            return Number(group.dataset.order);
        };
        Array.from(select.querySelectorAll('optgroup'))
            .sort(function (a, b) { return rank(a) - rank(b); })
            .forEach(function (group) { select.appendChild(group); });
        select.value = value;
    }

    function update(entry) {
        const employeeOption = entry.querySelector('.employee-select')?.selectedOptions[0];
        const specialty = employeeOption?.dataset.specialty || '';
        const isLaborer = specialty === LABORER;
        const isSalary = isLaborer && entry.querySelector('.laborer-unit-select')?.value === SALARY;

        entry.classList.toggle('is-laborer', isLaborer);
        entry.classList.toggle('is-salary', isSalary);
        entry.querySelectorAll(TOGGLED).forEach(function (element) {
            const disabled = !isShown(element, isLaborer, isSalary);
            const controls = element.matches('input, select') ? [element] : element.querySelectorAll('input, select');
            controls.forEach(function (control) { control.disabled = disabled; });
        });
        sortWorkTypes(entry.querySelector('.work-type-select'), specialty);
    }

    // В фазе перехвата: форма перестраивается раньше, чем обработчики страницы пересчитают суммы
    document.addEventListener('change', function (event) {
        if (event.target.matches('.employee-select, .laborer-unit-select')) {
            const entry = event.target.closest('.work-entry');
            if (entry) update(entry);
        }
    }, true);

    document.querySelectorAll('.work-entry').forEach(update);

    return { update: update };
})();
