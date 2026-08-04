# Distribution contract v1

This contract separates product identity, build lanes, payload composition, and
delivery. `build/lanes.json` remains the sole owner of compiler/runtime lane
facts. The TOML files under `release/profiles/` bind each active lane to one
delivery form and one canonical payload profile.

TOML is used for concise release-operator profiles. C3 parses a deliberately
small, strict subset: one top-level key/value assignment per line, lowercase
kebab-case keys, quoted ASCII strings, one integer schema version, no tables,
arrays, implicit values, duplicate keys, or unknown keys. The parser normalizes
each document into the JSON projection validated by
`distribution-profile.schema.json`.

`payload.schema.json` owns the language-neutral payload manifest. Build scripts
resolve its four source roots (`lane-output`, `cli-output`, `repository`, and
`generated`) and stage exactly those targets beneath the profile's versioned
archive root. Portable ZIP and future Universal Setup bindings must consume this
staged tree; they may not maintain another payload list.

Artifact status uses the doctrine vocabulary. During unpublished alphas the two
active profiles are `internal`; public beta/RC changes them to `preview`, and a
qualified stable identity changes them to `supported` through the normal version
transition and full requalification gate.
