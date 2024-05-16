document.querySelectorAll('.wish-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        var подарокId = this.getAttribute('data-подарокid');
        fetch(`/Home/ДобавитьЖелаемое?подарокId=${подарокId}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Ошибка добавления в желаемое');
                }
                // Дополнительная обработка данных при необходимости
            })
            .catch(error => {
                console.error(error);
            });
    });
});