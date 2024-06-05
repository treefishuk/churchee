window.enableQrCode = (element, uri) => {

    console.log("enable QR Code", element);
    console.log("enable QR Code", uri);

    try {

        new QRCode(element, {
            text: uri,
            width: 150,
            height: 150
        });

    } catch (e) {
        console.log("error", e);

    }


};