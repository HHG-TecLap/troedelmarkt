const express = require('express');
const app = express();

const { Sequelize, DataTypes, Model } = require('sequelize');
const sequelize = new Sequelize({ // Connect to DB
    dialect: 'sqlite',
    storage: 'data/database.sqlite'
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
        type: DataTypes.DECIMAL,
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
    return {name: obj['dataValues']['name'], id: obj['dataValues']['id'], balance: obj['dataValues']['balance']};
}

// register App routes
app.get('/', (req, res) => {
    res.send('Index')
});

// create a new Seller
app.get('/sellers', async (req, res) => {
    const sellers = await Seller.findAll();
    let filtered = [];
    sellers.forEach((seller, i, array) => {
        filtered.push(createResObj(seller))
    });
    res.send(filtered);
});

// get all Sellers
app.post('/seller', async (req, res) => {
    if (!req.body) return res.status(400).send('Bad Request');
    if (!req.body['name'] || !req.body['id']) return res.status(400).send('Bad Request');
    if (Seller.findAll({
        where: {
            id: req.body['id']
        }
    })) return res.status(409).send('A seller with this ID already exists');
    let seller = await Seller.create({name: req.body['name'], id: req.body['id'], balance: 0});
    res.json(createResObj(seller));
});

// get the Seller with the specified ID
app.get('/seller/:id', async (req, res) => {
    const seller = await Seller.findOne({
        where: {
            id: req.params['id']
        }
    });
    if (!seller) return res.status(400).send(`A seller with the id ${req.params['id']} doesn't exists`);
    return res.send(createResObj(seller));
});

app.delete('/seller/:id', async (req, res) => {
    const seller = await Seller.findOne({
        where: {
            id: req.params['id']
        }
    });
    if (!seller) return res.status(400).send(`A seller with the id ${req.params['id']} doesn't exists`);
    await seller.destroy();
    res.send(createResObj(seller));
});

app.listen(8080, () => {
    console.log('App runnling on http://127.0.0.1:8080')
});