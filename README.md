EFSpecs
=======

EFSpecs is a library to assist in testing your entity framework model mapping. It will create an entity with the values you provide it with, then send that entity off to the database, then pull it from the database to verify the values were saved correctly. And being the good citizen that it is, the whole thing is wrapped in a transaction so it all gets rolled back.

```CSharp
[Test]
public void VerifyContactsMappings()
{
  new PersistenceSpecification<Person>(() => new Context())
    .CheckProperty(x => x.Name, "Zaphod Beeblebrox")
    .CheckProperty(x => x.NumberOfHeads, 2)
    .CheckProperty(x => x.NumberOfArms, 3)
    .CheckReference(x => x.DistintCousins, new Person { Name = "Ford Prefect" })
    .VerifyMappings();
}
```

## Todo

I still have a few more things to accomplish with this library before I unleash it to the wild:

* Create more integration tests
* Refactor to be unit tested
* Add specific exceptions for known SQL errors such as invalid column, invalid object (table), etc
* Add better error messages for testers

## License

EFSpecs is released under the [MIT License](http://opensource.org/licenses/MIT).
