﻿using InventoryManagement.Api.Features.Authentication.Login;
using InventoryManagement.Api.Features.Authentication.ResetPassword;

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

    public static IEnumerable<string> ValidEmailSource()
    {
        yield return "alice.smith@example.com";
        yield return "john.doe123@company.co";
        yield return "mary.jane_98@domain.org";
        yield return "charlie+test@mail.net";
        yield return "emily.williams@web-service.info";
        yield return "david.brown99@service.biz";
        yield return "oliver_lee@site.co.uk";
        yield return "sophia.miller@address.us";
        yield return "james_bond007@movies.edu";
        yield return "mia.wilson@internet.club";
    }

    public static IEnumerable<string> ValidPasswordSource()
    {
        yield return "PaSsWoRd7";
        yield return "AbCdEfG123";
        yield return "TeStStr1ng";
        yield return "ComPlEx9";
        yield return "SeCurE01";
        yield return "VerIfY12";
        yield return "Pass123Word";
        yield return "TestStringA";
        yield return "SimPleEx8";
        yield return "ChEcKIt12";
    }

    public static IEnumerable<LoginInformation> ValidEmailAndPasswordSource()
    {
        using IEnumerator<string> emailEnumerator = ValidEmailSource().GetEnumerator();
        using IEnumerator<string> passwordEnumerator = ValidPasswordSource().GetEnumerator();

        while (emailEnumerator.MoveNext())
        {
            if (passwordEnumerator.MoveNext())
            {
                yield return new LoginInformation(emailEnumerator.Current, passwordEnumerator.Current);
            }
            else
            {
                yield break;
            }
        }
    }

    public static IEnumerable<PasswordResetTokenData> ValidPasswordResetDataSource()
    {
        using IEnumerator<string> emailSourceEnumerator = ValidEmailSource().GetEnumerator();
        using IEnumerator<string> passwordSourceEnumerator = ValidPasswordSource().GetEnumerator();
        using IEnumerator<string> resetTokenSourceEnumerator = ResetTokenSource().GetEnumerator();

        while (emailSourceEnumerator.MoveNext())
        {
            if (passwordSourceEnumerator.MoveNext() && resetTokenSourceEnumerator.MoveNext())
            {
                yield return new PasswordResetTokenData(emailSourceEnumerator.Current, resetTokenSourceEnumerator.Current, passwordSourceEnumerator.Current);
            }
            else
            {
                yield break;
            }
        }
    }

    private static IEnumerable<string> ResetTokenSource()
    {
        yield return "1245353";
        yield return "5etrer5de5";
        yield return "46rr5rr";
        yield return "6354erfge5";
        yield return "er654r4t";
        yield return "645343g";
        yield return "lab92vb";
        yield return "a549783";
        yield return "1074547433";
        yield return "nueludnaym4";
    }

    public static IEnumerable<string?> InvalidBatchNumberSource()
    {
        yield return "abcde12345";
        yield return "This!Is-A_Test";
        yield return "_START_HERE";
        yield return "lowercaseexample";
        yield return "ThisStringIsWayTooLong123456";
        yield return "HelloWorld!";
        yield return "Spaces Are Not Allowed";
        yield return "This#Has@Special&Chars";
        yield return "mixedCASE123";
        yield return "(Parentheses)AreNotValid";
        yield return "";
        yield return "    ";
        yield return null;
    }

    public static IEnumerable<string?> InvalidItemIdSource()
    {
        yield return "Hello World!";
        yield return "this_is_a_test";
        yield return "example_string";
        yield return "1234_example";
        yield return "_leading_underscore";
        yield return "trailing_underscore_";
        yield return "lowercase_only";
        yield return "Special@Chars!";
        yield return "Contains space";
        yield return "Upper and Lower";
        yield return "";
        yield return "     ";
        yield return null;
    }

    public static IEnumerable<decimal?> InvalidPerUnitPriceSource()
    {
        yield return -1.87m;
        yield return 0m;
        yield return -0.165m;
        yield return -45m;
        yield return null;
    }

    public static IEnumerable<int?> InvalidQuantitySource()
    {
        yield return 0;
        yield return -1;
        yield return -400;
        yield return int.MaxValue;
        yield return null;
    }

    public static IEnumerable<string> ValidBatchNumberSource()
    {
        yield return "HELLO_WORLD";
        yield return "USERNAME123";
        yield return "PASSWORD_42";
        yield return "ACCOUNT_789";
        yield return "PROFILE1";
        yield return "DATA_ENTRY2";
        yield return "TEST_CASE";
        yield return "VALIDATOR3";
        yield return "REPORT_2021";
        yield return "USERID_007";
    }

    public static IEnumerable<string> ValidItemNumberSource()
    {
        yield return "ALPHA_BRAVO";
        yield return "CHARLIE123";
        yield return "DELTA_789";
        yield return "ECHO_456";
        yield return "FOXTROT7";
        yield return "GOLF_001";
        yield return "HOTEL123";
        yield return "INDIA_9";
        yield return "JULIET8";
        yield return "KILO_MIKE";
    }

    public static IEnumerable<string> InvalidItemIdSourceWithoutNull()
    {
        yield return "Hello World!";
        yield return "this_is_a_test";
        yield return "example_string";
        yield return "1234_example";
        yield return "_leading_underscore";
        yield return "trailing_underscore_";
        yield return "lowercase_only";
        yield return "Special@Chars!";
        yield return "Contains space";
        yield return "Upper and Lower";
        yield return "";
        yield return "     ";
    }

    public static IEnumerable<string?> InvalidItemNameSource()
    {
        yield return "a1";
        yield return "X";
        yield return "nbfhdjfjsbfdsbfdfnstglqit64bx'7-cga21,b trnc=g5ras8";
        yield return "";
        yield return "     ";
        yield return null;
    }

    public static IEnumerable<string?> InvalidMeasurementUnitSource()
    {
        yield return "hgfhsfjsAnvybda";
        yield return "ADE1";
        yield return "517";
        yield return "";
        yield return "     ";
        yield return null;
    }

    public static IEnumerable<string> ValidItemNameSource()
    {
        yield return "Car";
        yield return "0.5 inch cables";
        yield return "Computer + Monitor";
        yield return "Oil";
        yield return "Gasoline";
        yield return "nbfhdjfjsbfdsbfdfnstglqit64bx'7-cga21,b trnc=g5ras";
        yield return "Salt";
        yield return "Wiring Equipment";
        yield return "Table fans";
        yield return "nhdfdf6f";
    }

    public static IEnumerable<string> ValidMeasurementUnitSource()
    {
        yield return "Meters";
        yield return "UNITS";
        yield return "MiliMeters";
        yield return "Grams";
        yield return "barfluvtsngtcph";
        yield return "A";
        yield return "Nos";
        yield return "yArds";
        yield return "litres";
        yield return "Each";
    }
}
