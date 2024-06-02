document.getElementById('apiButton').addEventListener('click', function () {
    window.location.href = '/swagger/index.html';
});


document.getElementById('updateStatisticsButton').addEventListener('click', function () {
    fetch('/api/StatisticApi/UpdateStatistics', { method: 'POST' })
        .then(response => response.json())
        .then(data => {
            document.getElementById('statisticsResult').textContent = data.message;
        })
        .catch(error => console.error(error));
});

// Обработчик нажатия на кнопку "Получить количество посещений"
document.getElementById('getUserVisitsButton').addEventListener('click', function () {
    fetch('/api/StatisticApi/GetUserVisitsThisMonth')
        .then(response => response.json())
        .then(data => {
            document.getElementById('visitsResult').textContent = 'Количество посещений в этом месяце: ' + data.totalVisits;
        })
        .catch(error => console.error(error));
});