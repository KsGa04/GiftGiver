document.querySelectorAll('.delete-wish').forEach(btn => {
    btn.addEventListener('click', function () {
        var подарокId = this.getAttribute('data-подарокid');
        fetch(`/Home/УдалитьЖелаемое?подарокId=${подарокId}`, {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => {
                if (!response.ok) {
                    throw new Error('Ошибка удаления желаемого');
                }
                else {
                    alert('Товар удален');
                    window.location.href = "/Home/PrivateAcc";
                }
                // Дополнительная обработка данных при необходимости
            })
            .catch(error => {
                console.error(error);
            });
    });
});