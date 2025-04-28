document.addEventListener("DOMContentLoaded", function () {
    let inventory = [];

    let tableBody = document.getElementById("warehouseTableBody");
    inventory.forEach(item => {
        let row = `<tr>
                        <td>${item.id}</td>
                        <td>${item.product}</td>
                        <td>${item.quantity}</td>
                        <td>${item.capacity}</td>
                        <td>${item.location}</td>
                   </tr>`;
        tableBody.innerHTML += row;
    });
});
