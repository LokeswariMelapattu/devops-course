const express = require('express'); 
const path = require('path');
const fs = require('fs');

const LOG_FILE = path.join(__dirname, 'storage.log');
const app = express();

const port = 8082;
 
app.use(express.text({ type: "*/*" })); // to Accept text/plain

fs.mkdirSync(path.dirname(LOG_FILE), { recursive: true });

function ensureLogFileExists() {
    if (!fs.existsSync(LOG_FILE)) {
        fs.writeFileSync(LOG_FILE, '');
    }
}

app.post('/log', (req, res) => {
    console.log('Received log:', req.body);
    ensureLogFileExists();
    console.log('Logging to file:', LOG_FILE);
    const logRecord = `${req.body}\n`;
    fs.appendFile(LOG_FILE, logRecord, (err) => {
        if (err) {
            console.error('Error writing to log file', err);
            return res.status(500).send('Internal Server Error');
        }
        res.status(200).send('Log added successfully');
    });
});

app.get('/log', (req, res) => { 
    fs.readFile(LOG_FILE, 'utf8', (err, data) => {
        if (err) {
            if (err.code === 'ENOENT') {
                return res.status(200).send(''); // No log file yet
            }
            console.error('Error reading log file', err);
            return res.status(500).send('Internal Server Error');
        }
        res.type('text/plain').send(data);
    });
});

app.listen(port, () => {
    console.log(`Storage service listening at http://localhost:${port}`);
});
