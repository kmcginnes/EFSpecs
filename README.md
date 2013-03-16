EFSpecs
=======

EFSpecs is a library to assist in testing your entity framework model mapping. It will create an entity with the values you provide it with, then send that entity off to the database, then pull it from the database to verify the values were saved correctly. And being the good citizen that it is, the whole thing is wrapped in a transaction so it all gets rolled back.
