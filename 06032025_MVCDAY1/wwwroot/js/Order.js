document.addEventListener("DOMContentLoaded", function () {
    let orders = [];

    let tableBody = document.getElementById("orderTableBody");
    orders.forEach(order => {
        let row = `<tr>
                        <td>${order.id}</td>
                        <td>${order.customer}</td>
                        <td>${order.total}</td>
                        <td>${order.status}</td>
                   </tr>`;
        tableBody.innerHTML += row;
    });
});
