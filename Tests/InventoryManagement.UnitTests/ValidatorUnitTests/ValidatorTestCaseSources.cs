using InventoryManagement.Api.Features.Authentication.Login;

namespace InventoryManagement.UnitTests.ValidatorUnitTests;
internal class ValidatorTestCaseSources
{
    public static IEnumerable<string?> InvalidEmailSource()
    {
        yield return @"plainaddress";
        yield return @"@missingusername.com";
        yield return @"username@.com";
        yield return @"username@com";
        yield return @"username@domain..com";
        yield return @"user@domain.com.";
        yield return @"user@domain,com";
        yield return @"user@domain com";
        yield return @"user@@domain.com";
        yield return @"user@domain.c";
        yield return @"";
        yield return @"    ";
        yield return null;
    }

    public static IEnumerable<string?> InvalidPasswordSource()
    {
        yield return @"lowercase";
        yield return @"UPPERCASE";
        yield return @"12345678";
        yield return @"Short";
        yield return @"Toolongforthisexample";
        yield return @"justlowercase";
        yield return @"ALLUPPERCASE";
        yield return @"1234abcd";
        yield return @"1234ABCD";
        yield return @"aBcDeFgHiJkLmNo.";
        yield return @"NoNumbersJustLetters";
        yield return @"spaces are bad";
        yield return @"punctuation!";
        yield return @"emptystring";
        yield return @"";
        yield return @"     ";
        yield return null;
    }

    public static IEnumerable<LoginInformation> ValidEmailAndPasswordSource()
    {
        yield return new LoginInformation("alice.smith@example.com", "PaSsWoRd7");
        yield return new LoginInformation("john.doe123@company.co", "AbCdEfG123");
        yield return new LoginInformation("mary.jane_98@domain.org", "TeStStr1ng");
        yield return new LoginInformation("charlie+test@mail.net", "ComPlEx9");
        yield return new LoginInformation("emily.williams@web-service.info", "SeCurE01");
        yield return new LoginInformation("david.brown99@service.biz", "VerIfY12");
        yield return new LoginInformation("oliver_lee@site.co.uk", "Pass123Word");
        yield return new LoginInformation("sophia.miller@address.us", "TestStringA");
        yield return new LoginInformation("james_bond007@movies.edu", "SimPleEx8");
        yield return new LoginInformation("mia.wilson@internet.club", "ChEcKIt12");
    }
}
