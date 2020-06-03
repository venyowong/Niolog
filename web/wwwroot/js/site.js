function formatDate(date) {
    return `${date.getFullYear()}-${date.getMonth() + 1}-${date.getDate()}`;
}

function formatTime(time) {
    if (!time) {
        return "00:00:00";
    }
    
    return `${time.getHours()}:${time.getMinutes()}:${time.getSeconds()}`;
}