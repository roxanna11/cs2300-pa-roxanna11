# PA3 - Graphics Transformation Engine

Complete the starter code for PA3.

## Files

- `Program.cs` (provided driver)
- `Pa3IO.cs` (provided reader and SVG writer)
- `Transforms.cs` (**your work**)
- `GraphicsEngine.csproj`
- `pa3_house.txt` (the assigned data file)

The data-file reader, SVG writer, and driver are **provided**. Do not modify them.

## Running the program

Requires the .NET 8 SDK.

```
cd CSharp
dotnet run                  # uses pa3_house.txt
dotnet run my_test.txt      # any other data file
```

The program reads `pa3_house.txt` unless you give it another data file. It prints every matrix and result and writes `pa3_output.svg`; open that file in any web browser to see the original object (gray, dashed) and the transformed object (blue).

## What you need to complete

Every method marked `TODO` in the transformations file:

| Method | What it must do |
|---|---|
| `RotateAbout` | Return `T(r) R T(-r)`, built from `Translate`, `Rotate`, and `MatMult`. |
| `ReflectLine` | Normalize the direction; throw `ArgumentException` for a zero direction. Return `T(q) F T(-q)` with `F = 2uu^T - I`. |
| `MapTriangle` | Return the 3x3 matrix with `A = V' V^-1` and translation column `b1 - A a1`; throw `ArgumentException` if `det V` is 0. |
| `Inverse` | Return the inverse by the block formula; throw `ArgumentException` if `det A` is 0. |
| `MaxDistance` | Return the largest distance between corresponding points of two lists. |

**The starter code also contains intentionally incorrect implementations.** Some methods that look finished are wrong. Use your hand work on the Mathematical Verification Form to find them, correct them, and verify the corrections.

## PA3 Repository and Submission Requirements

Use the **CS2300 programming repository you created in PA0**. Place these starter files in `ProgrammingAssignments/PA3`. Complete **one** language only.

### Programming Requirements
- Use the provided matrix multiplication (`MatMult`). Write your own inverse, using the block formula. **Do not** use NumPy, a matrix library, or another system to multiply, invert, or transform for you.
- The `Math` class is allowed (`Math.Sin`, `Math.Cos`, `Math.PI`, `Math.Sqrt`), and so is `double.Hypot`.
- Do not modify the provided data-file reader, SVG writer, or driver.
- Do not hard-code matrices, composite results, transformed coordinates, or errors. Everything must be computed from the data file.
- Compare decimal numbers with the tolerance (`1e-9`), never with `==`.
- Devise and run your own tests. Your program will also be run on instructor data files with other objects and other step sequences.
- Appropriately document code that you add or substantially modify, especially non-obvious mathematical/programming logic.

### Mathematical Verification
Download the PA3 Mathematical Verification Form separately from Canvas. Complete Parts 1-6 by hand **before beginning the programming portion** and show the mathematical work supporting every answer. Complete Part 7 after your program works.

Save the completed form as `PA3_LastName_FirstName_Verification.pdf` and place it in `ProgrammingAssignments/PA3` so it is included in the same Git commit as your completed program.

### Final Submission
Commit and push all required PA3 materials. Verify on GitHub that the submitted commit contains both your completed source code and completed verification PDF. Copy the repository URL and the **full commit hash**.

Download the fillable PA3 GitHub Submission Form from Canvas, complete it electronically, and save it as `PA3_LastName_FirstName_GitHubSubmission.pdf`. The Canvas form must be **typed in its entirety**.

If the repository URL or commit hash is missing, incorrect, inaccessible, or does not identify a commit containing the required PA3 materials, the PA3 grade is zero.
