# Dreamine.PLC.Omron.CxComponent

Omron CX-Compolet adapter boundary for Dreamine PLC communication.

## Important vendor runtime notice

This package must not redistribute Omron CX-Compolet, SYSMAC Gateway, DLLs, installers, samples, or licensed runtime files.

Users must install and license the required Omron software separately according to Omron's license terms.

This package may only contain adapter code that integrates with a user-installed vendor runtime.

## Current status

This package is a vendor runtime adapter boundary and is not part of the current simulator-validated protocol line.

Recommended production path:

- Use `Dreamine.PLC.Omron.Fins` for direct FINS TCP/UDP protocol communication.
- Use this package only when a project explicitly requires CX-Compolet or SYSMAC Gateway integration.

## License

Dreamine adapter code: MIT License.

Omron CX-Compolet and SYSMAC Gateway: not included and not licensed by this package.
