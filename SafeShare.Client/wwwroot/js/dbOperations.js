async function saveKeyPairInDatabase(userId, keyPair) {
    const db = await openDatabase();
    const transaction = db.transaction("keys", "readwrite");
    const store = transaction.objectStore("keys");

    const request = store.put({ id: userId, keyPair: keyPair });

    return new Promise((resolve, reject) => {
        request.onsuccess = () => resolve();
        request.onerror = (event) => reject('Error saving the key pair: ', event.target.errorCode);
    });
}

async function openDatabase() {
    return new Promise((resolve, reject) => {
        const request = indexedDB.open("cryptoKeysDatabase", 1);

        request.onerror = (event) => {
            reject('IndexedDB database error: ', event.target.errorCode);
        };

        request.onupgradeneeded = (event) => {
            const db = event.target.result;
            db.createObjectStore("keys", { keyPath: "id" });
        };

        request.onsuccess = (event) => {
            resolve(event.target.result);
        };
    });
}

async function getKeyPairFromDatabase(userId) {

    console.log("getKeyPairFromDatabase" + " " + userId);

    const db = await openDatabase();
    const transaction = db.transaction("keys", "readonly");
    const store = transaction.objectStore("keys");

    const request = store.get(userId);
    return new Promise((resolve, reject) => {
        request.onsuccess = (event) => {
            resolve(event.target.result.keyPair);
        };
        request.onerror = (event) => reject('Error retrieving the key pair: ', event.target.errorCode);
    });
}