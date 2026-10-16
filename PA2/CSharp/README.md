# PA2 - Campus Shuttle Flow Analyzer

Complete the starter code for PA2.

## Files

- `Program.cs`
- `CampusShuttleNetwork.cs`
- `MatrixTools.cs`
- `CampusShuttleFlow.csproj`

## Running the program

Requires the .NET 8 SDK.

```
cd CSharp
dotnet run
```

If your machine has a newer SDK but not .NET 8, change `<TargetFramework>` in `CampusShuttleFlow.csproj` to match (e.g. `net9.0`).

## What you need to complete

Every method marked `TODO` in the network file:

| Method | What it must do |
|---|---|
| `GetEquations` | Return one conservation equation per stop (5 total). |
| `BuildAugmentedMatrix` | Return the augmented matrix `[A b]` (coefficients plus the constant column): 5 rows (one per stop), 7 columns (x1..x6 plus the constant). |
| `ComputeGeneralSolution` | From the RREF, return an entry for **every** variable x1..x6, in order. Pivot variables map to an expression string (formatted like `"475 - x4"`, which is x1 in the handout's Figure 1 example); free variables map to the exact string `"free"`. |
| `ComputeFlowValues` | Given the RREF and the values the user typed for the free variables, return the numeric value of every variable x1..x6. |
| `IsFeasible` | Return true only if every route flow is nonnegative. |

The program asks for a value for **each** free variable, based on the `"free"` entries your general solution returns. Row reduction (`rref`) is already provided in the matrix tools file.

## PA2 Repository and Submission Requirements

Use the **CS2300 programming repository you created in PA0**. Place these starter files in `ProgrammingAssignments/PA2`. Complete **one** language only.

### Programming Requirements
- Use the provided row-reduction code. **Do not modify the provided row-reduction file.**
- Do not use an external library/system to perform the row reduction or solve the system for you.
- Do not hard-code RREF results, pivot/free-variable classifications, flow values, feasibility results, pivot columns, or the number of free variables.
- Devise and run your own tests. A separate student test file is not provided.
- Appropriately document code that you add or substantially modify, especially non-obvious mathematical/programming logic.

Your program may be evaluated using instructor test matrices/configurations in addition to the assigned campus shuttle network.

### Mathematical Verification
Download the PA2 Mathematical Verification Form separately from Canvas. Complete Parts 1-4 by hand **before beginning the programming portion** and show the mathematical work supporting every answer.

Save the completed form as `PA2_LastName_FirstName_Verification.pdf` and place it in `ProgrammingAssignments/PA2` so it is included in the same Git commit as your completed program.

### Final Submission
Commit and push all required PA2 materials. Verify on GitHub that the submitted commit contains both your completed source code and completed verification PDF. Copy the repository URL and the **full commit hash**.

Download the fillable PA2 GitHub Submission Form from Canvas, complete it electronically, and save it as `PA2_LastName_FirstName_GitHubSubmission.pdf`. The Canvas form must be **typed in its entirety**.

If the repository URL or commit hash is missing, incorrect, inaccessible, or does not identify a commit containing the required PA2 materials, the PA2 grade is zero.

