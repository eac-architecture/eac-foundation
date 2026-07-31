function repeat(character, count, result, cursor) {
    result = ""

    for (cursor = 0; cursor < count; cursor++) {
        result = result character
    }

    return result
}

function fit(value, width) {
    return length(value) <= width ? value : substr(value, 1, width - 3) "..."
}

function paint(value, color) {
    return use_color ? color value reset : value
}

function close_table() {
    if (!table_closed) {
        print border
        table_closed = 1
    }
}

function capture_summary(line, separator, key, value) {
    separator = index(line, ":")
    key = substr(line, 1, separator - 1)
    value = substr(line, separator + 1)
    gsub(/^[[:space:]]+|[[:space:]]+$/, "", key)
    gsub(/^[[:space:]]+|[[:space:]]+$/, "", value)
    summary[key] = value
}

BEGIN {
    status_width = 7
    rule_width = 20
    test_width = 60
    duration_width = 8
    escape = sprintf("%c", 27)
    green = escape "[32m"
    red = escape "[31m"
    yellow = escape "[33m"
    cyan = escape "[36m"
    bold = escape "[1m"
    reset = escape "[0m"
    border = "+-" repeat("-", status_width) \
        "-+-" repeat("-", rule_width) \
        "-+-" repeat("-", test_width) \
        "-+-" repeat("-", duration_width) "-+"

    print border
    header = sprintf( \
        "| %-*s | %-*s | %-*s | %-*s |", \
        status_width, "Status", \
        rule_width, "Rule", \
        test_width, "Test", \
        duration_width, "Duration")
    print paint(header, bold)
    print border
}

{
    sub(/\r$/, "", $0)
}

FILENAME != "-" {
    source_line = $0

    if (source_line ~ /^[[:space:]]*public[[:space:]]+(sealed[[:space:]]+)?class[[:space:]]+/) {
        class_name = source_line
        sub(/^.*class[[:space:]]+/, "", class_name)
        sub(/[^A-Za-z0-9_].*$/, "", class_name)
        current_class = class_name
    }

    if (source_line ~ /\[Trait\("Rule",[[:space:]]*"[^"]+"\)\]/) {
        split(source_line, trait_parts, "\"")
        pending_rule = trait_parts[4]
    }

    if (source_line ~ /\[(Fact|Theory)\(DisplayName[[:space:]]*=[[:space:]]*"[^"]+"\)\]/) {
        split(source_line, display_name_parts, "\"")
        pending_display_name = display_name_parts[2]
    }

    if (pending_rule != "" &&
        source_line ~ /^[[:space:]]*public[[:space:]]+(async[[:space:]]+)?(void|Task|ValueTask)[[:space:]]+[A-Za-z_][A-Za-z0-9_]*[[:space:]]*\(/) {
        method_name = source_line
        sub(/^[[:space:]]*public[[:space:]]+(async[[:space:]]+)?(void|Task|ValueTask)[[:space:]]+/, "", method_name)
        sub(/[[:space:]]*\(.*/, "", method_name)
        rule_by_test[current_class "." method_name] = pending_rule

        if (pending_display_name != "") {
            rule_by_test[pending_display_name] = pending_rule
        }

        pending_rule = ""
        pending_display_name = ""
    }

    next
}

/^(passed|failed|skipped) / {
    status = $1
    detail = $0
    sub(/^[^ ]+[[:space:]]+/, "", detail)
    duration = "-"

    if (match(detail, / \([^()]+\)$/)) {
        duration = substr(detail, RSTART + 2, RLENGTH - 3)
        detail = substr(detail, 1, RSTART - 1)
    }

    sub(/^EAC\.Foundation\.(UnitTests|ContractTests|ArchitectureTests)\./, "", detail)
    lookup_name = detail
    sub(/\(.*/, "", lookup_name)
    rule = rule_by_test[lookup_name]

    if (rule == "") {
        rule = "-"
    }

    if (status == "passed") {
        status = "PASS"
        status_color = green
    } else if (status == "failed") {
        status = "FAIL"
        status_color = red
    } else {
        status = "SKIP"
        status_color = yellow
    }

    status_cell = sprintf("%-*s", status_width, status)
    rule_cell = sprintf("%-*s", rule_width, fit(rule, rule_width))
    test_cell = sprintf("%-*s", test_width, fit(detail, test_width))
    duration_cell = sprintf("%*s", duration_width, duration)
    printf "| %s | %s | %s | %s |\n", \
        paint(status_cell, status_color), \
        paint(rule_cell, cyan), \
        test_cell, \
        duration_cell
    next
}

/^Running tests from / {
    next
}

/^[[:space:]]+from [A-Za-z]:\\/ {
    next
}

/^[[:space:]]*[A-Za-z]:\\.*\.dll .* (passed|failed|skipped) \(/ {
    next
}

/^Test run summary:/ {
    in_summary = 1
    next
}

in_summary && /^[[:space:]]+(total|failed|succeeded|skipped|duration):/ {
    capture_summary($0)

    if ($0 ~ /^[[:space:]]+duration:/) {
        close_table()
        summary_line = sprintf( \
            "Total: %s | Passed: %s | Failed: %s | Skipped: %s | Duration: %s", \
            summary["total"], \
            summary["succeeded"], \
            summary["failed"], \
            summary["skipped"], \
            summary["duration"])
        print paint(summary_line, summary["failed"] == "0" ? green : red)
    }

    next
}

NF > 0 {
    diagnostics = diagnostics $0 ORS
}

END {
    close_table()

    if (diagnostics != "") {
        print ""
        print "Additional details:"
        printf "%s", diagnostics
    }
}
