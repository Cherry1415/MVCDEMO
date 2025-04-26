
    function makePayment() {
            var amt =  $("#amountInput").val();
    var productId = $("#productIdInput").val();
    var quantity = parseInt($("#quantityInput").val());

    var amount = parseInt(amt);


    console.log(typeof(amount));

    fetch('/Payment/InitiateOrder', {
        method: 'POST',
    headers: {'Content-Type': 'application/json' },
    body: JSON.stringify({Amount: amount,productId: productId, quantity: quantity,orderItems: [
    {
        ProductId: parseInt(productId),
    Quantity: quantity,
    Price: amount
            }] 
                })
            })
            .then(response => response.json())
            .then(data => {
                if (data.orderId) {
                    var options = {
        "key": "rzp_test_A6DIgBxiN6cygo",
    "amount": amount * 100,
    "currency": "INR",
    "name": "Your Company",
    "description": "Test Payment",
    "order_id": data.orderId,
    "handler": function (response) {
        console.log("Payment Response:", response);
    var paymentData = {
        razorpay_payment_id: response.razorpay_payment_id,
    razorpay_order_id: response.razorpay_order_id,
    razorpay_signature: response.razorpay_signature,
    amount: amount,
                        };
    fetch('/Payment/Success', {
        method: 'POST',
    headers: {
        'Content-Type': 'application/json',
                            },
    body: JSON.stringify(paymentData),
                        })
                        .then(res => res.json())
                        .then(response => {
        console.log("Success Response:", response);
    if (response.success) {
        alert('Payment successful!');
    // Optionally, redirect to the order success page or confirmation page
    window.location.href = '/Payment/ThankYou';
                            } else {
        alert('Payment verification failed');
                            }
                        });
                        },
    "prefill": {
        "name": "Test User",
    "email": "test@example.com",
    "contact": "9909817574"
                        }
                    };

    var rzp1 = new Razorpay(options);
    rzp1.open();
                } else {
        alert('Error: ' + data.error);
                }
            });
        }
