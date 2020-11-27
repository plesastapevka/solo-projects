const express = require("express");
const ObjectID = require("mongoose").Types.ObjectID;
const router = express.Router();
const ContactModel = require("../models/contactModel");
const bcrypt = require("bcrypt");
const saltRounds = 10;
const jwt = require("jsonwebtoken");
require("dotenv").config();

// PUT Update or create a contact
router.put("/:id", async (req, res) => {
    let body = req.body;
    let contactId = req.params.id;
    let userId = req.body.userId;
    let token = req.body.accessToken;

    if (!userId) return res.status(500).send({ message: "There was an error uploading contact." });

    if (!token) return res.status(403).send({ message: "Unauthorized!" });

    await jwt.verify(token, process.env.PRIVATE_KEY, (err, decoded) => {
        if (err) return res.status(403).send({ message: "Unauthorized!" });

        if (err) {
            return res.status(500).send({ message: "There was an error uploading contact." });
        }

        // Set up new contact model
        let newContact = {
            userId: userId,
            name: body.name,
            lastName: body.lastName,
            phoneNumber: body.phoneNumber,
            email: body.email
        }

        // let contact = ContactModel.findOne({ userId: body.userId, ObjectID: contactId });
        if(contactId == 0) {
            // Create
            ContactModel.create(newContact, (err, updatedContact) => {
                if (err) return res.status(500).send(err);

                return res.status(200).send({ message: "Contact created sucessfully!", contact: updatedContact });
            })
        } else {
            // Update
            ContactModel.findByIdAndUpdate(contactId, newContact, { new: true, upsert: true, setDefaultsOnInsert: true }, (err, updatedContact) => {
                if (err) return res.status(500).send(err);
                
                return res.status(200).send({ message: "Contact updated sucessfully!", contact: updatedContact });
            })
        }
    })
});

// GET all contacts
router.get("/", async (req, res) => {
    let token = req.headers["x-access-token"];
    let userId = req.query.userId;

    if(!token) return res.status(403).send({ message: "Unauthorized!" });

    await jwt.verify(token, process.env.PRIVATE_KEY, async (err, decoded) => {
        if(err) return res.status(403).send({ message: "Unauthorized!"});

        await ContactModel.find({ userId: userId }, (err, contacts) => {
            if (err) return res.status(500).send({ message: "Error fetching database." });
            if(contacts.length == 0) return res.status(404).send({ message: "No contacts found." });

            return res.status(200).send(contacts);
        });
    })
});

// GET contact by ID
router.get("/:id", async (req, res) => {
    let token = req.headers["x-access-token"];
    let contactId = req.params.id;
    if(!token) return res.status(403).send({ message: "Unauthorized!" });

    await jwt.verify(token, process.env.PRIVATE_KEY, async (err, decoded) => {
        if(err) return res.status(403).send({ message: "Unauthorized!" });

        let contact = await ContactModel.findById(contactId);

        if(!contact) return res.status(404).send({ message: "No contacts found." });
        
        return res.status(200).send(contact);
    })
});

// POST delete contact
router.post("/", async (req, res) => {
    let token = req.headers["x-access-token"];
    let contactId = req.body.id;
    if(!token) return res.status(403).send({ message: "Unauthorized!"});

    await jwt.verify(token, process.env.PRIVATE_KEY, async (err, decode) => {
        if(err) return res.status(403).send({ message: "Unauthorized!" });
    })

    let contact = await ContactModel.findById(contactId);
    if(!contact) return res.status(404).send({ message: "No contact found." });

    await contact.deleteOne((err) => {
        if(err) return res.status(500).send({ message: "Contact could not be deleted." });
    })

    return res.status(200).send({ message: "Contact deleted." });
});

module.exports = router;