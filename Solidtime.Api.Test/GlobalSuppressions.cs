// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
	"Naming",
	"CA1707:Identifiers should not contain underscores",
	Justification = "Fine for unit tests"
)]
[assembly: SuppressMessage(
	"Performance",
	"CA1848:Use the LoggerMessage delegates",
	Justification = "Performance is not critical in test code"
)]
[assembly: SuppressMessage(
	"Design",
	"CA1031:Do not catch general exception types",
	Justification = "Test cleanup should be best-effort and not fail the test"
)]

// The following suppressions are for Codacy's Lizard tool which flags methods over 50 lines.
// CRUD integration tests are intentionally comprehensive, covering create, read, update, and delete
// operations in a single test to ensure proper cleanup and to test the full lifecycle.
// Splitting these tests would reduce test reliability and make cleanup more complex.
