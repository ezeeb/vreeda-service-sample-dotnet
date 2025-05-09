import { DeviceRequestModel } from "@/types/vreedaApi";
import { Alert, Box, Button, Card, CardContent, Link, TextField, Typography } from "@mui/material";
import { useState } from "react";

export default function CustomPatternControl({selectedDevices}: {selectedDevices: string[]}) {
    const [error, setError] = useState<string | null>(null);
    const [pattern, setPattern] = useState("type:football|f:3|x:9.3|r:0.1,0,1.0;0.1,1.0,0;0.1,0,1.0;0.7,1.0,0;0.2,0,0;0.1,0,1.0;0.1,1.0,0;0.1,0,1.0;0.2,1.0,0;0.3,0,0.0;0.1,0.0,0;0.2,0,0.0;0.4,0.0,0;0.1,0,1.0;0.1,1.0,0;0.1,0,1.0;0.2,1.0,0;0.3,0,1.0;0.1,1.0,0;0.2,0,1.0;0.4,1.0,0;0.1,0,1.0;0.1,1.0,0;0.1,0,1.0;0.7,1.0,0;0.3,0,1.0;0.1,1.0,0;0.2,0,1.0;0.4,1.0,0;0.2,0,1.0;0.9,1.0,0;2,0,0|g:0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.7,0.0,0;0.2,0,0;0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.2,0.0,0;0.3,0,1.0;0.1,1.0,0;0.2,0,1.0;0.4,1.0,0;0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.2,0.0,0;0.3,0,0.0;0.1,0.0,0;0.2,0,0.0;0.4,0.0,0;0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.7,0.0,0;0.3,0,0.0;0.1,0.0,0;0.2,0,0.0;0.4,0.0,0;0.2,0,0.0;0.9,0.0,0;2,0,0|b:0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.7,0.0,0;0.2,0,0;0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.2,0.0,0;0.3,0,0.0235;0.1,0.0235,0;0.2,0,0.0235;0.4,0.0235,0;0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.2,0.0,0;0.3,0,0.0;0.1,0.0,0;0.2,0,0.0;0.4,0.0,0;0.1,0,0.0;0.1,0.0,0;0.1,0,0.0;0.7,0.0,0;0.3,0,0.0;0.1,0.0,0;0.2,0,0.0;0.4,0.0,0;0.2,0,0.0;0.9,0.0,0;2,0,0|c:0.1,0,0;0.1,0,0.15;0.3,0.15,0;0.5,0,0;0.2,0,0;0.2,0,0;0.1,0,0.15;0.2,0.15,0;0.4,0,0;0.1,0,0.15;0.2,0.15,0;0.3,0,0;0.2,0,0;0.1,0,0.15;0.2,0.15,0;0.4,0,0;0.1,0,0.15;0.2,0.15,0;0.3,0,0;0.1,0,0;0.1,0,0.15;0.3,0.15,0;0.5,0,0;0.4,0,0;0.1,0,0.15;0.2,0.15,0;0.3,0,0;1.1,0,0;2,0,0");

    const handleRunClicked = async () => {
        console.log("patching devices: ", selectedDevices);
        const request: DeviceRequestModel = {
            states: {
                pattern: pattern,
                program: 'custom',
            }
        };
        setError(null);
        if (selectedDevices.length === 0) {
            setError('no device selected');
        } else {
            selectedDevices.forEach(async (deviceId) => {
                try {
                    const response = await fetch('/api/vreeda/patch-device', {
                        method: 'PATCH',
                        headers: {
                            'Content-Type': 'application/json',
                        },
                        body: JSON.stringify({ deviceId, request }),
                    });
                
                    if (!response.ok) {
                        throw new Error('Failed to update device');
                    }
                
                    const result = await response.json();
                    if (!result.success) {
                        throw new Error(result.error || 'Failed to update device');
                    }
                    console.log('Device updated successfully:', result.data);
                    return result.data;
                } catch (error) {
                    setError((error as Error).message);
                    console.log('Error running pattern: ', error);
                    throw error;
                }
            });
        }
    };

    return (
        <Box sx={{ pb: 4 }}>
            <Card variant="outlined" sx={{ p: 2, backgroundColor: '#2A1D24' }}>
                <CardContent>
                    <Box display="flex" flexDirection="column" gap={2}>
                        <TextField
                            label="Custom Pattern"
                            variant="outlined"
                            multiline
                            rows={10}
                            value={pattern}
                            onChange={(e) => setPattern(e.target.value)}
                            fullWidth
                            InputLabelProps={{
                            sx: { color: "white" }, // Optional: style the label
                            }}
                            sx={{
                            input: { color: "white" }, // Optional: style the text
                            }}
                        />
                        <Typography variant="body2" color="text.secondary">
                            More information on custom patterns on{" "}
                            <Link href="https://api.vreeda.com/" target="_blank" rel="noopener noreferrer">
                                https://api.vreeda.com/
                            </Link>
                        </Typography>
                        <Button
                            variant="contained"
                            color="primary"
                            onClick={handleRunClicked}
                        >
                            Run
                        </Button>
                        {error && <Alert severity="error">Error: {error}</Alert>}
                    </Box>
                </CardContent>
            </Card>
        </Box>
    );
}