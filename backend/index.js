const express = require('express');
const app = express();
const fs = require('fs');
const Decimal = require('decimal.js');

const config = JSON.parse(fs.readFileSync('./config.json', 'utf8'));

const { Sequelize, DataTypes, Model } = require('sequelize');
const sequelize = new Sequelize({ // Connect to DB
    dialect: 'sqlite',
    storage: 'data/database.sqlite',
    password: config['dbPassword']
});

// Test DB connection (only works in an async-Context)
(async () => {
    try {
        await sequelize.authenticate();
        console.log('Connection has been established successfully.');
    } catch (error) {
        console.error('Unable to connect to the database:', error);
    }
})();


// Define DB Model
class Seller extends Model {};

Seller.init({
    name: {
        type: DataTypes.STRING,
        allowNull: false
    },
    id: {
        type: DataTypes.STRING,
        allowNull: false,
        primaryKey: true
    },
    balance: {
        type: DataTypes.STRING,
        allowNull: false
    },
    rate: {
        type: DataTypes.STRING,
        allowNull: false
    }
}, {
    sequelize, 
    modelName: 'Seller'
});

(async () => {
    await sequelize.sync();
    console.log('Synced the DB')
})();

app.use(express.json());
app.use(express.urlencoded({ extended: true }));

// helper Functions
/**
 * Function for creating the Response Object (DB object without createdAt and updatedAt)
 * @param {object} obj The DB Object
 * @returns {object} The Response Object
 */
function createResObj(obj) {
    const balance = new Decimal(obj['dataValues']['balance']);
    const rate = new Decimal(obj['dataValues']['rate']);

    const provision = balance.times(rate);
    const revenue = balance.minus(provision)
    return {name: obj['dataValues']['name'], id: obj['dataValues']['id'], balance: obj['dataValues']['balance'], rate: obj['dataValues']['rate'], provision: provision.toString(), revenue: revenue.toString()}; // ToDo: Provision ausrechnen (provision), Händler Geld (revenue)
}

// register App routes
app.get('/', (req, res) => {
    res.send('Index')
});

// get all Sellers
app.get('/sellers', async (req, res) => {
    const sellers = await Seller.findAll();
    let filtered = [];
    sellers.forEach((seller, i, array) => {
        filtered.push(createResObj(seller))
    });
    res.send(filtered);
});

// create a new Seller 
app.post('/seller', async (req, res) => {
    if (!req.body) return res.status(400).send('Bad Request');
    if (!req.body['name'] || !req.body['id'] || !req.body['rate']) return res.status(400).send('Bad Request');
    if (Seller.findAll({
        where: {
            id: req.body['id']
        }
    })) return res.status(409).send('A seller with this ID already exists');
    let seller = await Seller.create({name: req.body['name'], id: req.body['id'], balance: '0', rate: req.body['rate']});
    res.status(201).json(createResObj(seller));
});

// get the Seller with the specified ID
app.get('/seller/:id', async (req, res) => {
    const seller = await Seller.findOne({
        where: {
            id: req.params['id']
        }
    });
    if (!seller) return res.status(404).send(`A seller with the id ${req.params['id']} doesn't exists`);
    return res.send(createResObj(seller));
});

app.delete('/seller/:id', async (req, res) => {
    const seller = await Seller.findOne({
        where: {
            id: req.params['id']
        }
    });
    if (seller.dataValues['balance'])
    if (!seller) return res.status(404).send(`A seller with the id ${req.params['id']} doesn't exists`);
    await seller.destroy();
    res.send(createResObj(seller));
});

app.get('/teapot', async (req, res) => {
    res.status(418).send('I\'m a teapot! \n I refuse the attempt to brew coffee with a teapot');
});

app.listen(8080, () => {
    console.log('App runnling on http://127.0.0.1:8080')
});