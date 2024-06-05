const express = require('express');
const app = express();

const receivedData = [];

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

app.get('/api/values/:id', function(req, res) {
    const id = parseInt(req.params.id);
    const item = receivedData.find((item) => item.id === id);

    if (item) {
      res.send(item.data);
    }
    res.end();
});

app.put('/api/values/:id', function(req, res) {
    const id = parseInt(req.params.id);
    const data = req.body;

    const item = receivedData.find((item) => item.id === id);
    if (item) {
      item.data = {...data};
    } else {
      receivedData.push({id: id, data: {...data}});
    }
    res.end();
});


app.listen(5000);