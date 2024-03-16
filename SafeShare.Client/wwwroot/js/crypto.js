class KeyPairs {
    constructor(publicKey, privateKey) {
        this.publicKey = publicKey;
        this.privateKey = privateKey;
    }
}

async function hashPasswordAndUserID(password, userId) {
    const combined = `${password}:${userId}`;
    const encoder = new TextEncoder();
    const data = encoder.encode(combined);

    const hashBuffer = await crypto.subtle.digest('SHA-256', data);
    return new Uint8Array(hashBuffer);
}

async function generateKeyFromPasswordAndUserID(password, userId) {
    const seed = await hashPasswordAndUserID(password, userId);
    const keyPair = nacl.box.keyPair.fromSecretKey(seed.slice(0, 32));

    const publicKeyBase64 = nacl.util.encodeBase64(keyPair.publicKey);
    const secretKeyBase64 = nacl.util.encodeBase64(keyPair.secretKey);

    return new KeyPairs(publicKeyBase64, secretKeyBase64);
}

async function encryptExpense(expense, receiverPublicKeyBase64, senderSecretKeyBase64) {

    const nonce = nacl.randomBytes(nacl.box.nonceLength);

    const encryptedTitle = await encryptDataWithNonce(expense.title, receiverPublicKeyBase64, senderSecretKeyBase64, nonce);
    const encryptedAmount = await encryptDataWithNonce(expense.amount.toString(), receiverPublicKeyBase64, senderSecretKeyBase64, nonce);
    const encryptedDescription = await encryptDataWithNonce(expense.description, receiverPublicKeyBase64, senderSecretKeyBase64, nonce);

    const encryptedExpense = {
        GroupId: expense.groupId,
        Date: expense.date,
        Title: encryptedTitle,
        Amount: expense.Amount,
        Description: encryptedDescription,
        Nonce: nacl.util.encodeBase64(nonce),
        EncryptedAmount: encryptedAmount
    };

    console.log(encryptedExpense);
    console.log(expense);

    return encryptedExpense;
}

async function decryptExpense
(
    encryptedExpense,
    receiverSecretKeyBase64,
    senderPublicKeyBase64
)
{
    const nonce = nacl.util.decodeBase64(encryptedExpense.Nonce);

    const decryptedTitle = await decryptDataWithNonce(encryptedExpense.Title, receiverSecretKeyBase64, senderPublicKeyBase64, nonce);
    console.log()

    const decryptedAmount = await decryptDataWithNonce(encryptedExpense.Amount, receiverSecretKeyBase64, senderPublicKeyBase64, nonce);
    console.log()

    const decryptedDescription = await decryptDataWithNonce(encryptedExpense.Description, receiverSecretKeyBase64, senderPublicKeyBase64, nonce);
    console.log()

    const decryptedExpense = {
        GroupId: encryptedExpense.GroupId,
        Date: encryptedExpense.Date,
        Title: decryptedTitle,
        Amount: parseFloat(decryptedAmount),
        Description: decryptedDescription,
    };

    return decryptedExpense;
}

async function encryptDataWithNonce
    (
        encryptData,
        receiverPublicKeyBase64,
        senderSecretKeyBase64,
        nonce
    ) {
    const encryptDataUint8 = nacl.util.decodeUTF8(encryptData);
    const receiverPublicKey = nacl.util.decodeBase64(receiverPublicKeyBase64);
    const senderSecretKey = nacl.util.decodeBase64(senderSecretKeyBase64);

    const encryptedMessage = nacl.box(encryptDataUint8, nonce, receiverPublicKey, senderSecretKey);

    return nacl.util.encodeBase64(encryptedMessage);
}

async function decryptDataWithNonce(encryptedData, receiverSecretKeyBase64, senderPublicKeyBase64, nonce) {

    const encryptedDataDecoded = nacl.util.decodeBase64(encryptedData);
    const receiverSecretKey = nacl.util.decodeBase64(receiverSecretKeyBase64);
    const senderPublicKey = nacl.util.decodeBase64(senderPublicKeyBase64);

    const decryptedMessage = nacl.box.open(encryptedDataDecoded, nonce, senderPublicKey, receiverSecretKey);

    if (!decryptedMessage)
        throw new Error('Could not decrypt the message.');

    return nacl.util.encodeUTF8(decryptedMessage);
}
